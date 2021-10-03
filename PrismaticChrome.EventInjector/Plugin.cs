using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HttpServer;
using LazyUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using OTAPI;
using Terraria;
using TerrariaApi.Server;

namespace PrismaticChrome.EventInjector
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {
        internal static TerrariaPlugin Instance;
        
        public Plugin(Main game) : base(game)
        {
            Instance = this;
        }

        private static readonly Regex codeReg = new Regex(@"/\* ref:(.*?) \*/", RegexOptions.Compiled | RegexOptions.Singleline);
        private const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static object GetMember(object @base, Type type, string part)
        {
            var prop = type.GetProperty(part, flags);
            if (prop != null)
            {
                return prop.GetValue(@base);
            }
            var field = @base.GetType().GetField(part, flags);
            if (field != null)
            {
                return prop.GetValue(@base);
            }
            throw new InvalidOperationException();
        }

        public override void Initialize()
        {
            
            Dictionary<string, Action<MethodInfo>> registrator = new Dictionary<string, Action<MethodInfo>>();
            var refs = ServerApi.Plugins.Select(p => p.Plugin.GetType().Assembly)
                .Concat(Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select(Assembly.Load)).Distinct().ToArray();

            var code = new StringBuilder();
            var usings = new []
            {
                "OTAPI",
                "Terraria",
                "TerrariaApi.Server",
                "TShockAPI",
                "LazyUtils",
                "System",
                "System.Collections.Generic",
                "System.Text.RegularExpressions",
                "System.Linq"
            };
            foreach (var @using in usings)
                code.Append($"using {@using};\n");
            foreach (var pair in Config.Instance.events)
            {
                var parts = pair.Key.Split(':');
                var type = refs.Select(a => a.GetType(parts[0])).First(t => t != null);
                object @base = null;
                foreach (var part in parts.Skip(1).Take(parts.Length - 2))
                {
                    @base = GetMember(@base, type, part);
                    type = @base.GetType();
                }

                var part2 = parts.Last();
                var prop2 = @base.GetType().GetProperty(part2, flags);
                var field2 = @base.GetType().GetField(part2, flags);
                var instance = prop2 == null ? field2.GetValue(@base) : prop2.GetValue(@base);
                var setter = prop2 == null ? new Action<object>(o => field2.SetValue(@base, o)) : o => prop2.SetValue(@base, o);
                var register = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .FirstOrDefault(m => m.GetParameters().Length == 2 && m.Name == "Register");
                var funckey = "Class" + Math.Abs(pair.Key.GetHashCode());
                if (register == null)
                {
                    registrator.Add(funckey, d => setter(Delegate.Combine((Delegate)instance, d.CreateDelegate(instance.GetType()))));
                }
                else
                {
                    registrator.Add(funckey, d => register.Invoke(instance, new object[]{this, d.CreateDelegate(register.GetParameters()[1].ParameterType)}));
                }

                code.AppendLine($"public static class {funckey} {{{File.ReadAllText(pair.Value)}}}");

                var codeText = code.ToString();
                
                var references = Directory.GetFiles(".")
                    .Concat(Directory.GetFiles("ServerPlugins")).Where(n => n.EndsWith(".dll", true, CultureInfo.CurrentCulture)
                      || n.EndsWith(".exe", true, CultureInfo.CurrentCulture)).Select(r => MetadataReference.CreateFromFile(r))
                    .Concat(refs.Select(r => r.Location).Where(r => !string.IsNullOrEmpty(r)).Select(r => MetadataReference.CreateFromFile(r)))
                    .Distinct().Where(r =>
                    {
                        try
                        {
                            Assembly.LoadFrom(r.FilePath);
                        }
                        catch
                        {
                            return false;
                        }

                        return true;
                    });
                File.WriteAllText("debug.cs", codeText);

                var syntaxTree = SyntaxFactory.ParseSyntaxTree(codeText, new CSharpParseOptions(LanguageVersion.Latest), "code.cs", Encoding.UTF8);

                var compilation = CSharpCompilation.Create("Reply", new [] { syntaxTree }, references, new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    false,
                    null, null, null, null, OptimizationLevel.Release,
                    false, true, null, null, default, null, Platform.AnyCpu,
                    ReportDiagnostic.Default, 4, null, true, false, null, null, null, DesktopAssemblyIdentityComparer.Default,
                    null, false, MetadataImportOptions.Public
                ));

                using (var ms = new MemoryStream())
                {
                    var result = compilation.Emit(ms);

                    if (!result.Success)
                    {
                        var msg = string.Join("\n",

                            result.Diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Warning).Select(diagnostic =>
                            {
                                FileLinePositionSpan lineSpan = diagnostic.Location.GetLineSpan();
                                return new CompilerError
                                {
                                    ErrorNumber = diagnostic.Id,
                                    IsWarning = (diagnostic.Severity == DiagnosticSeverity.Warning),
                                    ErrorText = diagnostic.GetMessage(null),
                                    FileName = (lineSpan.Path ?? ""),
                                    Line = lineSpan.StartLinePosition.Line + 1,
                                    Column = lineSpan.StartLinePosition.Character
                                }.ToString();
                            }));
                        throw new Exception(msg);
                    }

                    var asm = Assembly.Load(ms.ToArray());

                    foreach (var pair2 in registrator)
                    {
                        pair2.Value(asm.GetType(pair2.Key).GetMethod("Handler"));
                    }
                }

            }
        }
    }
}
