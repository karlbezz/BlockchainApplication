using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainApplication.Data.Objects
{
    public class Wallet
    {
        public string PrivateKey;
        public string PublicKey;

        public Wallet()
        {
            SecureRandom secureRandom = new SecureRandom();
            X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");
            ECDomainParameters domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
            ECKeyPairGenerator generator = new ECKeyPairGenerator();
            ECKeyGenerationParameters keygenParams = new ECKeyGenerationParameters(domain, secureRandom);
            generator.Init(keygenParams);
            AsymmetricCipherKeyPair keypair = generator.GenerateKeyPair();

            this.PrivateKey = Hex.ToHexString(PrivateKeyInfoFactory.CreatePrivateKeyInfo(keypair.Private).ParsePrivateKey().GetDerEncoded());
            this.PublicKey = Hex.ToHexString(((ECPublicKeyParameters)keypair.Public).Q.GetEncoded(true));

        }
    }
}
