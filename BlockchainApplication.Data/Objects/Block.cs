using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Objects
{
    public class Block
    {
        public string Hash { get; set; }
        public HashedContent HashedContent { get; set; }

        public Block()
        {
            HashedContent = new HashedContent();
        }

        public Block(HashedContent hashedContent)
        {
            this.HashedContent = hashedContent;
        }
    }
}
