using System.Collections;

namespace Woof.Algorithms {

    /// <summary>
    /// Hash code calculation tool for custom comparers and such.
    /// </summary>
    /// <remarks>
    /// Since .NET Standard 2.1 there is framework struct with conflicting name:
    /// See <see href="https://docs.microsoft.com/en-us/dotnet/api/system.hashcode?view=netstandard-2.1">.NET Standard 2.1 documentation.</see>
    /// </remarks>
    public sealed class HashCode {

        /// <summary>
        /// Initializes <see cref="HashCode"/> calculator with primes.
        /// </summary>
        /// <param name="collectionPrime">A prime number used to calculate codes for collections.</param>
        /// <param name="componentPrime">A prime number used to calculate codes for components.</param>
        public HashCode(int collectionPrime = 17, int componentPrime = 31) {
            CollectionPrime = collectionPrime;
            ComponentPrime = componentPrime;
        }

        /// <summary>
        /// Calculates the hash code value for any collection of primitive objects.
        /// </summary>
        /// <param name="collection">Any collection of primitive objects.</param>
        /// <returns>Hash code value.</returns>
        public int GetFromCollection(IEnumerable collection) {
            int value = 0;
            foreach (var item in collection) value = unchecked(CollectionPrime * value + item.GetHashCode());
            return value;
        }

        /// <summary>
        /// Calculates the hash code value for any primitives or collections of primitives components.
        /// </summary>
        /// <param name="components">Primitives or collections of primitives.</param>
        /// <returns>Hash code value.</returns>
        public int GetFromComponents(params object[] components) {
            int value = 0;
            for (int i = 0, n = components.Length; i < n; i++)
                value = unchecked(
                    ComponentPrime * value + (
                        components[i] is IEnumerable
                            ? GetFromCollection(components[i] as IEnumerable)
                            : components[i].GetHashCode()
                    )
                );
            return value;
        }

        /// <summary>
        /// A prime number used to calculate hash codes for collections.
        /// </summary>
        private readonly int CollectionPrime;

        /// <summary>
        /// A prime number used to calculate hash codes for components.
        /// </summary>
        private readonly int ComponentPrime;

    }

}