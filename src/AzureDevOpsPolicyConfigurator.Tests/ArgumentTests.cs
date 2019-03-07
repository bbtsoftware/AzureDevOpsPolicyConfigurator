using System;
using System.IO;
using AzureDevOpsPolicyConfigurator.Exceptions;
using Xunit;

namespace AzureDevOpsPolicyConfigurator.Tests
{
    /// <summary>
    /// Argument tester
    /// </summary>
    public class ArgumentTests
    {
        [Fact(DisplayName = "Test null arguments")]
        private async void TestRunWithNullArgument()
        {
            Assert.Equal(0, await new Program().Run(null, true).ConfigureAwait(false));
        }

        [Fact(DisplayName = "Test without arguments")]
        private async void TestRunWithoutArgument()
        {
            Assert.Equal(0, await new Program().Run(Array.Empty<string>(), true).ConfigureAwait(false));
        }

        [Fact(DisplayName = "Test without collection url")]
        private async void TestNoCollectionUrl()
        {
            var args = new string[1] { "generate" };

            var ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"connectionurl\" is missing!", ex.Message);
        }

        [Fact(DisplayName = "Test without auth")]
        private async void TestNoAuth()
        {
            var args = new string[3] { "generate", "-c", "url" };

            var ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"auth\" is missing!", ex.Message);
        }

        [Fact(DisplayName = "Test with basic auth but no user")]
        private async void TestBasicButNoUser()
        {
            var args = new string[5] { "generate", "-c", "url", "-a", "basic" };

            var ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"user\" must be provided if \"auth\" is set to Basic or OAuth!", ex.Message);
        }

        [Fact(DisplayName = "Test with OAuth auth but no user")]
        private async void TestOAuthButNoUser()
        {
            var args = new string[5] { "generate", "-c", "url", "-a", "oauth" };

            var ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"user\" must be provided if \"auth\" is set to Basic or OAuth!", ex.Message);
        }

        [Fact(DisplayName = "Test with basic auth but no password")]
        private async void TestBasicButNoPassword()
        {
            var args = new string[7] { "generate", "-c", "url", "-a", "basic", "-u", "user" };

            var ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"password\" must be provided if \"auth\" is set to Basic or OAuth!", ex.Message);
        }

        [Fact(DisplayName = "Test with OAuth auth but no user")]
        private async void TestOAuthButNoPassword()
        {
            var args = new string[7] { "generate", "-c", "url", "-a", "oauth", "-u", "user" };

            var ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"password\" must be provided if \"auth\" is set to Basic or OAuth!", ex.Message);
        }

        [Fact(DisplayName = "Test without input file and as whatif")]
        private async void TestWithoutInputFile()
        {
            var args = new string[9] { "whatif", "-c", "url", "-a", "oauth", "-u", "user", "-p", "password" };

            var ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);

            Assert.NotNull(ex);
            Assert.IsType<ArgumentValidationException>(ex);
            Assert.Equal("Argument \"in\" must be provided!", ex.Message);
        }

        [Fact(DisplayName = "Test with input but file missing")]
        private async void TestWithNoFile()
        {
            var args = new string[7] { "whatif", "-c", "url", "-a", "ntlm", "--in", ".\\file.json" };

            var ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);

            Assert.NotNull(ex);
            Assert.IsType<FileNotFoundException>(ex);
        }

        [Fact(DisplayName = "Test parameter resolving")]
        private async void TestParameterResolving()
        {
            var args = new string[11] { "generate", "-c", "url", "-a", "oauth", "-u", "user", "-p", "pass", "-d", "dest" };
            var ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);
            Assert.NotNull(ex);
            Assert.IsType<UriFormatException>(ex);

            args = new string[11] { "generate", "--collectionurl", "url", "--auth", "oauth", "--user", "user", "--password", "pass", "--desctination", "dest" };
            ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);
            Assert.NotNull(ex);
            Assert.IsType<UriFormatException>(ex);

            args = new string[9] { "whatif", "-c", "url", "-a", "ntlm", "-i", ".\\file.json", "-v", "info" };
            ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);
            Assert.NotNull(ex);
            Assert.IsType<FileNotFoundException>(ex);

            args = new string[9] { "execute", "-c", "url", "-a", "ntlm", "--in", ".\\file.json", "--verbosity", "info" };
            ex = await Record.ExceptionAsync(() => new Program().Run(args, true)).ConfigureAwait(false);
            Assert.NotNull(ex);
            Assert.IsType<FileNotFoundException>(ex);
        }
    }
}
