using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;

namespace Icebreaker.Controllers
{
    public class PsaEncyrpt
    {
        string PublicKey;
        string PrivateKey;
        byte[] EncryteData;
        byte[] DecryptData;
        string EncryteStr;
        string DecryptStr;

        public PsaEncyrpt()
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            PublicKey = "<RSAKeyValue><Modulus>sE5kwn0PHuzq1FRqBUeJM6gDY8aA+92+yGF5P0+mhzSo1awHt+jHgsiqoeaO5Bk6jVcP" +
                     "+CWE9vbMjCE4ex47oWR0CPOHLlwouYSgQvRym8NhN3J0zy6WRzgIGe2Z5Yu6NDKYPxCydpQsg2vEINzWCD4KpFIgMTIJ400uZ9R1Nq0=</Modulus>" +
                     "<Exponent>AQAB</Exponent><P>7ABlhu+4ee/qy7NMUzgopDKj+7r7dPq99P4ciiVss3AyG54Yogyckm2bdAxWqzHmCH2gN+KkMQc//3p/UOqcaQ==</P>" +
                     "<Q>vz8FVGot99h0htjerJdyzCBf99H02kKEEUr61b30BAUvm5O0RZbdD7HMxCSQ4/T407TLiLys94SgyeRk9+pPpQ==</Q>" +
                     "<DP>WS4JL4VUo5dalWEKnYPiL7IHL2/H57t8nqCLrlRYEDJ8bN1AF7RGUjri/GZRNd3kPB8ktRmKzBAeSe9DPxN4yQ==</DP><DQ>W16" +
                     "xni4IfkQlEBPm9xB4YTwKfa0KzYg/7r8i7iGNxqnvn+XGmATG4uuwh/lsW" +
                     "+y7QPI817xE6xCrfSmob6W7nQ==</DQ><InverseQ>Vwula6rZB5gvxn7AjIbX2g0/Azme5NuOdE2pgaIcMtmq6BdnmfuEzV/jty/y" +
                     "+j1zkq8LQ0B8a9yXBMCyOMTXsQ==</InverseQ><D>LWFwVBNmofovk8nKpGM" +
                     "+cJptjPAaYTo5klBsqhwxbBnk32LxdagoOoS2TwgOfa30wU7IoIHf0MXD7snaRO6KQFmmro7gkONGO9EqFOR2VG4MeLrQqjBpNIoSdXDOqzAMKv" +
                     "VycC4bWoZfU0IaQiS50ngJ9L2XI5Ot6K3O6zg0YSE=</D></RSAKeyValue>";

            PrivateKey = "<RSAKeyValue><Modulus>sE5kwn0PHuzq1FRqBUeJM6gDY8aA+92+yGF5P0+mhzSo1awHt+jHgsiqoeaO5Bk6jVcP" +
                       "+CWE9vbMjCE4ex47oWR0CPOHLlwouYSgQvRym8NhN3J0zy6WRzgIGe2Z5Yu6NDKYPxCydpQsg2vEINzWCD4KpFIgMTIJ400uZ9R1Nq0=</Modulus>" +
                       "<Exponent>AQAB</Exponent><P>7ABlhu+4ee/qy7NMUzgopDKj+7r7dPq99P4ciiVss3AyG54Yogyckm2bdAxWqzHmCH2gN+KkMQc//3p/UOqcaQ==</P>" +
                       "<Q>vz8FVGot99h0htjerJdyzCBf99H02kKEEUr61b30BAUvm5O0RZbdD7HMxCSQ4/T407TLiLys94SgyeRk9+pPp" +
                       "Q==</Q><DP>WS4JL4VUo5dalWEKnYPiL7IHL2/H57t8nqCLrlRYEDJ8bN1AF7RGUjri/GZRNd3kPB8ktRmKzBAeSe9DPxN4yQ==</DP><DQ>W16" +
                       "xni4IfkQlEBPm9xB4YTwKfa0KzYg/7r8i7iGNxqnvn+XGmATG4uuwh/lsW" +
                       "+y7QPI817xE6xCrfSmob6W7nQ==</DQ><InverseQ>Vwula6rZB5gvxn7AjIbX2g0/Azme5NuOdE2pgaIcMtmq6BdnmfuEzV/jty/y" +
                       "+j1zkq8LQ0B8a9yXBMCyOMTXsQ==</InverseQ><D>LWFwVBNmofovk8nKpGM" +
                       "+cJptjPAaYTo5klBsqhwxbBnk32LxdagoOoS2TwgOfa30wU7IoIHf0MXD7snaRO6KQFmmro7gkONGO9EqFOR2VG4MeLrQqjBpNIoSdXDOqzAMKv" +
                       "VycC4bWoZfU0IaQiS50ngJ9L2XI5Ot6K3O6zg0YSE=</D></RSAKeyValue>";
        }

        /// <summary>
        /// 加密函数
        /// </summary>
        /// <returns></returns>
        public string Encrypt(string Data)
        {
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            rsa.FromXmlString(PublicKey);//拿到公钥加密

            byte[] data = System.Text.Encoding.Default.GetBytes(Data); //Data转换Byte[] 数组  步骤A
            EncryteData = rsa.Encrypt(data, false);//加密

            EncryteStr = Convert.ToBase64String(EncryteData);//Byte[] 转Base64string以便数据库存储

           return EncryteStr;
        }

        /// <summary>
        /// 解密函数
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public string Decrypt(string Data)
        {
            byte[] a;
            System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            rsa.FromXmlString(PrivateKey);//拿到私钥

            a = Convert.FromBase64String(Data);//Base64string转Byte[]数组

            DecryptData = rsa.Decrypt(a, false);//解密
           
            DecryptStr = System.Text.Encoding.Default.GetString(DecryptData);//Byte[]数组转string类型(和步骤A相反)
            return DecryptStr;
        }
    }
}