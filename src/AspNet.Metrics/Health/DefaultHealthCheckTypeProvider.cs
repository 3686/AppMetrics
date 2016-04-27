using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AspNet.Metrics.Infrastructure;

namespace AspNet.Metrics.Health
{
    public class DefaultHealthCheckTypeProvider : IHealthCheckTypeProvider
    {
        private const string HealthCheckTypeName = "HealthCheck";
        private static readonly TypeInfo ObjectTypeInfo = typeof(object).GetTypeInfo();
        private readonly IMetricsAssemblyProvider _assemblyProvider;

        public DefaultHealthCheckTypeProvider(IMetricsAssemblyProvider assemblyProvider)
        {
            _assemblyProvider = assemblyProvider;
        }

        /// <inheritdoc />
        public virtual IEnumerable<TypeInfo> HealthCheckTypes
        {
            get
            {
                var candidateAssemblies = new HashSet<Assembly>(_assemblyProvider.CandidateAssemblies);
                var types = candidateAssemblies.SelectMany(a => a.DefinedTypes);
                return types.Where(typeInfo => IsHealthCheck(typeInfo, candidateAssemblies));
            }
        }

        protected internal virtual bool IsHealthCheck(
            TypeInfo typeInfo,
            ISet<Assembly> candidateAssemblies)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            if (candidateAssemblies == null)
            {
                throw new ArgumentNullException(nameof(candidateAssemblies));
            }

            if (!typeInfo.IsClass)
            {
                return false;
            }
            if (typeInfo.IsAbstract)
            {
                return false;
            }
            // We only consider public top-level classes as health check. IsPublic returns false for nested
            // classes, regardless of visibility modifiers
            if (!typeInfo.IsPublic)
            {
                return false;
            }
            if (typeInfo.ContainsGenericParameters)
            {
                return false;
            }
            if (!typeInfo.Name.EndsWith(HealthCheckTypeName, StringComparison.OrdinalIgnoreCase) &&
                !DerivesFromHealthCheck(typeInfo, candidateAssemblies))
            {
                return false;
            }
            if (typeInfo.IsDefined(typeof(ObsoleteAttribute)))
            {
                return false;
            }

            return true;
        }

        private bool DerivesFromHealthCheck(TypeInfo typeInfo, ISet<Assembly> candidateAssemblies)
        {
            while (typeInfo != ObjectTypeInfo)
            {
                var baseTypeInfo = typeInfo.BaseType.GetTypeInfo();

                // A base type will be treated as a health check if
                // a) it ends in the term "HealthCheck" and
                // b) it's assembly is one of the candidate assemblies we're considering. This ensures that the assembly
                // the base type is declared in references metrics.
                if (baseTypeInfo.Name.EndsWith(HealthCheckTypeName, StringComparison.Ordinal) &&
                    candidateAssemblies.Contains(baseTypeInfo.Assembly))
                {
                    return true;
                }

                // c). The base type is called 'HealthCheck.
                if (string.Equals(baseTypeInfo.Name, HealthCheckTypeName, StringComparison.Ordinal))
                {
                    return true;
                }

                typeInfo = baseTypeInfo;
            }

            return false;
        }
    }
}