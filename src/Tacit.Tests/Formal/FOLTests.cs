using System.Collections.Generic;
using Tacit.Formal.FirstOrderLogic;
using Xunit;

namespace Tacit.Tests.Formal;

public class FOLTests {
    [Fact]
    public void TestRuleMatch1() {
        var betterRule = new FOLRuleExpression(
            new FOLRule("better", new[] {
                "?x", "?y"
            })
        );
        var cakeBetter = new FOLFact("better", new[] {
            "cake", "pie"
        });
        var kb = new FOLKnowledgeBase(new List<FOLFact> {
            cakeBetter
        });
        var context1 = new FOLMatchContext();
        var isMatch = betterRule.Matches(kb, context1);
        Assert.True(isMatch);
        Assert.Equal("cake", context1.Get("?x"));
        Assert.Equal("pie", context1.Get("?y"));
    }

    [Fact]
    public void TestKnowledge1() {
        var kb = new FOLKnowledgeBase(new List<FOLFact> {
            new FOLFact("yummy", new[] {
                "cake"
            }),
        });

        // ensure cake is yummy
        Assert.True(kb.Ask(new FOLFact("yummy", new[] {
            "cake"
        })));
        // check if salad is yummy
        Assert.False(kb.Ask(new FOLFact("yummy", new[] {
            "salad"
        })));
    }

    [Fact]
    public void TestTransitive1() {
        // abc_data = [ 'a beats b', 'b beats c' ]
        var kb = new FOLKnowledgeBase(
            new List<FOLFact> {
                new FOLFact("beats", new[] {
                    "a", "b"
                }),
                new FOLFact("beats", new[] {
                    "b", "c"
                })
            }
        );
        var rules = new List<FOLRuleExpression> {
            // beats(?x, ?y) & beats(?y, ?z) -> beats(?x, ?z)
            new FOLConditional(
                Antecedent: new FOLIfExpression(new FOLAndExpression(
                    new FOLRuleExpression[] {
                        new FOLRule("beats", new[] {
                            "?x", "?y"
                        }),
                        new FOLRule("beats", new[] {
                            "?y", "?z"
                        })
                    }
                )),
                Consequent: new FOLThenExpression(new FOLRuleExpression(
                    new FOLRule("beats", new[] {
                        "?x", "?z"
                    })
                ))
            )
        };
        var prover = new FOLProver();
        prover.ForwardChain(rules, kb);

        // we should have produced beats(a, c)
        Assert.Equal(3, kb.Facts.Count);
        Assert.Contains(new FOLFact("beats", new[] {
            "a", "c"
        }), kb.Facts);
    }

    [Fact]
    public void TestParserSimpleFact() {
        var parser = new FOLParser();
        var fact1 = parser.ParseFact("beats(a, b)");
        Assert.Equal("beats", fact1.Predicate);
        Assert.Equal(2, fact1.Referents.Length);
        Assert.Equal("a", fact1.Referents[0]);
        Assert.Equal("b", fact1.Referents[1]);
    }
}