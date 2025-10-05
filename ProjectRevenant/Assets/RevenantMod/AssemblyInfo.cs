using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

[assembly:InternalsVisibleTo("RevenantMod.Editor")]
[assembly:HG.Reflection.SearchableAttribute.OptIn]

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618
[module: UnverifiableCode]