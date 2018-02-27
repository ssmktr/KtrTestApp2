using UnityEngine;
using System;
using System.Security;
using System.Collections;
using System.Security.Cryptography;

public class RSACryptUtil
{
    public static string encryptText(string strPlainText, string modulus, string pubExponent)
    {
        return Convert.ToBase64String(RSACryptUtil.encrypt(strPlainText, modulus, pubExponent));
    }

    public static byte[] encrypt(string strPlainText, string modulus, string pubExponent)
    {
        //Create a new instance of the RSACryptoServiceProvider class.
        RSACryptoServiceProvider rsaObj = new RSACryptoServiceProvider(512);

        //Create a new instance of the RSAParameters structure.
        RSAParameters rsaPars = new RSAParameters();

        rsaPars.Modulus = Convert.FromBase64String(modulus);
        rsaPars.Exponent = Convert.FromBase64String(pubExponent);

        //Import key parameters into RSA.
        rsaObj.ImportParameters(rsaPars);

        byte[] bytText = new byte[strPlainText.Length];

        for (int i = 0; i < strPlainText.Length; i++)
        {
            bytText[i] = Convert.ToByte(strPlainText[i]);
        }

        byte[] bytEncText = rsaObj.Encrypt(bytText, false);

        return (bytEncText);
    }
}
