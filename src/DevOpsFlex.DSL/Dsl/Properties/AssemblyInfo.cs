#region Using directives

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

#endregion

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle(@"")]
[assembly: AssemblyDescription(@"")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(@"Microsoft")]
[assembly: AssemblyProduct(@"WPFDesigner")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: System.Resources.NeutralResourcesLanguage("en")]

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion(@"1.0.0.0")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]

//
// Make the Dsl project internally visible to the DslPackage assembly
//
[assembly: InternalsVisibleTo(@"Microsoft.WPFDesigner.DslPackage, PublicKey=00240000048000009400000006020000002400005253413100040000010001002D5ED7AB91EB908B6769C14C9F9D7DE2EE6A41DED0274CAB8FA1F1F618F9324D4864AD1092A6A6EC3F420FC3E7D2F57A142A8AE18BD40BDEC9F9F67ED7B64EC55EED791650DF58FCA2BFC8546913410822D70CE20D2F441C9507D7B60BCED265AC17FF3D7C20935834F80C03896DFC0B3EC147A59BFC4A86F5B1CA38947FCEE4")]