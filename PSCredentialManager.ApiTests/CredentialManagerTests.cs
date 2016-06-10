using System;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSCredentialManager.Api;
using PSCredentialManager.Api.Fakes;
using PSCredentialManager.Common;
using PSCredentialManager.Common.Enum;

namespace PSCredentialManager.ApiTests
{
    [TestClass()]
    public class CredentialManagerTests
    {
        private static CredentialManager manager;
        public CredentialManagerTests()
        {
            manager = new CredentialManager();
        }

        [TestMethod()]
        public void WriteCredTest()
        {
            
            using (ShimsContext.Create())
            {
                ShimImports.CredWriteNativeCredentialRefUInt32 =
                   (ref NativeCredential credential, UInt32 flags) => 
                {
                    return true;
                };

                manager.WriteCred(new NativeCredential());
            }
        }

        [TestMethod()]
        public void ReadCredTest()
        {
            using (ShimsContext.Create())
            {
                ShimImports.CredReadStringCredTypeInt32IntPtrOut =
                    (string target, CredType type, int flag, out IntPtr credentialPointer) =>
                    {
                        credentialPointer = new IntPtr();
                        return true;
                    };

                ShimCriticalCredentialHandle.ConstructorIntPtr =
                    (CriticalCredentialHandle credentialHandle, IntPtr credentialPointer) =>
                    {

                    };

                ShimCriticalCredentialHandle.AllInstances.GetCredential =
                    (CriticalCredentialHandle criticalCredentialHandle) =>
                    {
                        return new Credential();
                    };

                manager.ReadCred("server01", CredType.Generic);
            }
        }

        [TestMethod()]
        public void DeleteCredTest()
        {
            using (ShimsContext.Create())
            {
                ShimImports.CredDeleteStringCredTypeInt32 =
                    (string target, CredType type, int flag) =>
                    {
                        return true;
                    };

                manager.DeleteCred("server01", CredType.Generic);
            }
        }

        [TestMethod()]
        public void ReadCredTest1()
        {
            using (ShimsContext.Create())
            {
                ShimImports.CredEnumerateStringInt32Int32OutIntPtrOut =
                    (string filter, int flags, out int count, out IntPtr credentialPointer) =>
                    {
                        count = 1;
                        credentialPointer = new IntPtr();
                        return true;
                    };

                ShimCriticalCredentialHandle.ConstructorIntPtr =
                    (CriticalCredentialHandle credentialHandle, IntPtr credentialPointer) =>
                    {

                    };

                ShimCriticalCredentialHandle.AllInstances.GetCredentialsInt32 =
                    (CriticalCredentialHandle credentialHandle, int count) =>
                    {
                        return new Credential[count];
                    };

                manager.ReadCred();
            }
        }
    }
}