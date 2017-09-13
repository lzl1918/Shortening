using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shortening.Models;

namespace Shortening.Services
{
    public interface IAliasProvider
    {
        string GetAlias();
    }

    public sealed class AliasProvider : IAliasProvider
    {
        public const string CHARACTERS = "abcdefghijklmnopqrstuvwxyz0123456789";
        private readonly MapDbContext dbContext;
        private readonly Random random;

        public AliasProvider(MapDbContext dbContext)
        {
            this.random = new Random(Guid.NewGuid().GetHashCode());
            this.dbContext = dbContext;
        }

        public string GetAlias()
        {
            bool exists;
            string alias = null;
            StringBuilder aliasBuilder = new StringBuilder(capacity: 5);
            aliasBuilder.Append("     ");
            int setSize = CHARACTERS.Length;
            byte[] hashBytes;
            do
            {
                hashBytes = BitConverter.GetBytes(Guid.NewGuid().GetHashCode());

                aliasBuilder[0] = GetCharacter(hashBytes[0]);
                aliasBuilder[1] = GetCharacter(hashBytes[1]);
                aliasBuilder[2] = GetCharacter(hashBytes[2]);
                aliasBuilder[3] = GetCharacter(hashBytes[3]);
                aliasBuilder[4] = GetCharacter(random.Next(127));
                alias = aliasBuilder.ToString();
                exists = dbContext.Items.FirstOrDefault(item => item.Alias == alias) != null;
            } while (exists);
            return alias;
            char GetCharacter(int val)
            {
                while (val >= setSize) val %= setSize;
                return CHARACTERS[val];
            };
        }
    }
}
