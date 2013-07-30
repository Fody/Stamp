using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using LibGit2Sharp;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class TokenResolverTests
{
    private ModuleDefinition _moduleDefinition;
    private FormatStringTokenResolver _resolver;

    [TestFixtureSetUp]
    public void FixtureSetUp()
    {
        var beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        _moduleDefinition = ModuleDefinition.ReadModule(beforeAssemblyPath);

        _resolver = new FormatStringTokenResolver();
    }

    private void DoWithCurrentRepo(Action<Repository> doWithRepo)
    {
        using (var repo = new Repository(GitDirFinder.TreeWalkForGitDir(Environment.CurrentDirectory)))
        {
            if (doWithRepo != null) doWithRepo(repo);
        }
    }

    [Test]
    public void Replace_version()
    {
        DoWithCurrentRepo(repo =>
            {
                var result = _resolver.ReplaceTokens("%version%", _moduleDefinition, repo);

                Assert.AreEqual("1.0.0.0", result);
            });
    }

    [Test]
    public void Replace_version1()
    {
        DoWithCurrentRepo(repo =>
            {
                var result = _resolver.ReplaceTokens("%version1%", _moduleDefinition, repo);

                Assert.AreEqual("1", result);
            });
    }

    [Test]
    public void Replace_version2()
    {
        DoWithCurrentRepo(repo =>
            {
                var result = _resolver.ReplaceTokens("%version2%", _moduleDefinition, repo);

                Assert.AreEqual("1.0", result);
            });
    }

    [Test]
    public void Replace_version3()
    {
        DoWithCurrentRepo(repo =>
            {
                var result = _resolver.ReplaceTokens("%version3%", _moduleDefinition, repo);

                Assert.AreEqual("1.0.0", result);
            });
    }

    [Test]
    public void Replace_version4()
    {
        DoWithCurrentRepo(repo =>
            {
                var result = _resolver.ReplaceTokens("%version4%", _moduleDefinition, repo);

                Assert.AreEqual("1.0.0.0", result);
            });
    }

    [Test]
    public void Replace_branch()
    {
        DoWithCurrentRepo(repo =>
            {
                var branchName = repo.Head.Name;

                var result = _resolver.ReplaceTokens("%branch%", _moduleDefinition, repo);

                Assert.AreEqual(branchName, result);
            });
    }

    [Test]
    public void Replace_githash()
    {
        DoWithCurrentRepo(repo =>
            {
                var sha = repo.Head.Tip.Sha;

                var result = _resolver.ReplaceTokens("%githash%", _moduleDefinition, repo);

                Assert.AreEqual(sha, result);
            });
    }

    [Test]
    public void Replace_haschanges()
    {
        DoWithCurrentRepo(repo =>
            {
                var isDirty = repo.Index.Count > 0;

                var result = _resolver.ReplaceTokens("%haschanges%", _moduleDefinition, repo);

                if (isDirty)
                {
                    Assert.AreEqual("HasChanges", result);
                }
                else
                {
                    Assert.AreEqual(string.Empty, result);
                }
            });
    }
}