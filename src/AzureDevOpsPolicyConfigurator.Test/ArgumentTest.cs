using System;
using System.IO;
using AzureDevOpsPolicyConfigurator.Exceptions;
using Xunit;

namespace AzureDevOpsPolicyConfigurator.Test
{
    /// <summary>
    /// Argument tester
    /// </summary>
    public class ArgumentTest
    {
        [Fact(DisplayName = "Test null arguments")]
        private void TestRunWithNullArgument()
        {
            Assert.Equal(0, new Program().Run(null, true));
        }

        [Fact(DisplayName = "Test without arguments")]
        private void TestRunWithoutArgument()
        {
            Assert.Equal(0, new Program().Run(Array.Empty<string>(), true));
        }

        [Fact(DisplayName = "Test without collection url")]
        private void TestNoCollectionUrl()
        {
            var args = new string[1] { "generate" };

            var ex = Record.Exception(() => new Program().Run(args, true));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"connectionurl\" is missing!", ex.Message);
        }

        [Fact(DisplayName = "Test without auth")]
        private void TestNoAuth()
        {
            var args = new string[3] { "generate", "-c", "url" };

            var ex = Record.Exception(() => new Program().Run(args, true));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"auth\" is missing!", ex.Message);
        }

        [Fact(DisplayName = "Test with basic auth but no user")]
        private void TestBasicButNoUser()
        {
            var args = new string[5] { "generate", "-c", "url", "-a", "basic" };

            var ex = Record.Exception(() => new Program().Run(args, true));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"user\" must be provided if \"auth\" is set to Basic or OAuth!", ex.Message);
        }

        [Fact(DisplayName = "Test with OAuth auth but no user")]
        private void TestOAuthButNoUser()
        {
            var args = new string[5] { "generate", "-c", "url", "-a", "oauth" };

            var ex = Record.Exception(() => new Program().Run(args, true));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"user\" must be provided if \"auth\" is set to Basic or OAuth!", ex.Message);
        }

        [Fact(DisplayName = "Test with basic auth but no password")]
        private void TestBasicButNoPassword()
        {
            var args = new string[7] { "generate", "-c", "url", "-a", "basic", "-u", "user" };

            var ex = Record.Exception(() => new Program().Run(args, true));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"password\" must be provided if \"auth\" is set to Basic or OAuth!", ex.Message);
        }

        [Fact(DisplayName = "Test with OAuth auth but no user")]
        private void TestOAuthButNoPassword()
        {
            var args = new string[7] { "generate", "-c", "url", "-a", "oauth", "-u", "user" };

            var ex = Record.Exception(() => new Program().Run(args, true));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"password\" must be provided if \"auth\" is set to Basic or OAuth!", ex.Message);
        }

        [Fact(DisplayName = "Test without input file and as whatif")]
        private void TestWithoutInputFile()
        {
            var args = new string[9] { "whatif", "-c", "url", "-a", "oauth", "-u", "user", "-p", "password" };

            var ex = Record.Exception(() => new Program().Run(args, true));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"in\" must be provided!", ex.Message);
        }

        [Fact(DisplayName = "Test with input but file missing")]
        private void TestWithNoFile()
        {
            var args = new string[7] { "whatif", "-c", "url", "-a", "ntlm", "--in", ".\\file.json" };

            var ex = Record.Exception(() => new Program().Run(args, true));

            Assert.NotNull(ex);
            Assert.IsType<FileNotFoundException>(ex);
        }

        [Fact(DisplayName = "Test parameter resolving")]
        private void TestParameterResolving()
        {
            var args = new string[11] { "generate", "-c", "url", "-a", "oauth", "-u", "user", "-p", "pass", "-d", "dest" };
            var ex = Record.Exception(() => new Program().Run(args, true));
            Assert.NotNull(ex);
            Assert.IsType<UriFormatException>(ex);

            args = new string[11] { "generate", "--collectionurl", "url", "--auth", "oauth", "--user", "user", "--password", "pass", "--desctination", "dest" };
            ex = Record.Exception(() => new Program().Run(args, true));
            Assert.NotNull(ex);
            Assert.IsType<UriFormatException>(ex);

            args = new string[7] { "whatif", "-c", "url", "-a", "ntlm", "-i", ".\\file.json" };
            ex = Record.Exception(() => new Program().Run(args, true));
            Assert.NotNull(ex);
            Assert.IsType<FileNotFoundException>(ex);

            args = new string[7] { "execute", "-c", "url", "-a", "ntlm", "--in", ".\\file.json" };
            ex = Record.Exception(() => new Program().Run(args, true));
            Assert.NotNull(ex);
            Assert.IsType<FileNotFoundException>(ex);
        }
    }
}
