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
    public void TestConjunction1() {
        var kb = new FOLKnowledgeBase(new List<FOLFact> {
            new FOLFact("likes", new[] {
                "alice", "pie"
            }),
            new FOLFact("likes", new[] {
                "bob", "pie"
            }),
        });
        var bothLikePieRule = new FOLRuleExpression(
            new FOLAndExpression(new FOLRuleExpression[] {
                new FOLRule("likes", new[] {
                    "alice", "pie"
                }),
                new FOLRule("likes", new[] {
                    "bob", "pie"
                }),
            })
        );
        var context1 = new FOLMatchContext();
        var isMatch = bothLikePieRule.Matches(kb, context1);
        Assert.True(isMatch);
    }

    [Fact]
    public void TestConjunction2() {
        // a conjunction using variables
        var kb = new FOLKnowledgeBase(new List<FOLFact> {
            new FOLFact("likes", new[] {
                "alice", "pie"
            }),
            new FOLFact("likes", new[] {
                "alice", "cake"
            }),
        });
        var personLikesPieAndCake = new FOLRuleExpression(
            new FOLAndExpression(new FOLRuleExpression[] {
                new FOLRule("likes", new[] {
                    "?person", "pie"
                }),
                new FOLRule("likes", new[] {
                    "?person", "cake"
                }),
            })
        );
        var context1 = new FOLMatchContext();
        var isMatch = personLikesPieAndCake.Matches(kb, context1);
        Assert.True(isMatch);
        Assert.Equal("alice", context1.Get("?person"));
    }

    [Fact]
    public void TestConditional1() {
        // if alice likes pie, then alice likes cake
        var kb = new FOLKnowledgeBase(new List<FOLFact> {
            new FOLFact("likes", new[] {
                "alice", "dessert"
            }),
        });
        var rules = new List<FOLRuleExpression> {
            new FOLConditional(
                Antecedent: new FOLIfExpression(new FOLRule("likes", new[] {
                    "?person", "dessert"
                })),
                Consequent: new FOLThenExpression(new FOLRule("likes", new[] {
                    "?person", "cake"
                }))
            )
        };
        var prover = new FOLProver();
        prover.ForwardChain(rules, kb);

        // we should have produced likes(alice, cake)
        Assert.True(kb.Ask(new FOLFact("likes", new[] {
            "alice", "cake"
        })));
    }

    [Fact]
    public void TestTransitive1() {
        var kb = new FOLKnowledgeBase(
            new List<FOLFact> {
                new FOLFact("greater", new[] {
                    "ten", "five"
                }),
                new FOLFact("greater", new[] {
                    "five", "one"
                })
            }
        );
        var rules = new List<FOLRuleExpression> {
            // beats(?x, ?y) & beats(?y, ?z) _> beats(?x, ?z)
            new FOLConditional(
                Antecedent: new FOLIfExpression(new FOLAndExpression(
                    new FOLRuleExpression[] {
                        new FOLRule("greater", new[] {
                            "?x", "?y"
                        }),
                        new FOLRule("greater", new[] {
                            "?y", "?z"
                        })
                    }
                )),
                Consequent: new FOLThenExpression(new FOLRuleExpression(
                    new FOLRule("greater", new[] {
                        "?x", "?z"
                    })
                ))
            )
        };
        var prover = new FOLProver();
        prover.ForwardChain(rules, kb);

        // we should have produced greater(ten, one)
        Assert.Equal(3, kb.Facts.Count);
        Assert.True(kb.Ask(new FOLFact("greater", new[] {
            "ten", "one"
        })));
    }

    [Fact]
    public void TestParserSimpleFact() {
        var parser = new FOLParser();
        var fact1 = parser.ParseFact("beats(paper, rock)");
        Assert.Equal("beats", fact1.Predicate);
        Assert.Equal(2, fact1.Referents.Length);
        Assert.Equal("paper", fact1.Referents[0]);
        Assert.Equal("rock", fact1.Referents[1]);
    }

    [Fact]
    public void TestParserFactList() {
        var parser = new FOLParser();
        var facts = parser.ParseFacts("beats(paper, rock); beats(rock, scissors)");
        Assert.Equal(2, facts.Count);
        Assert.Equal("beats", facts[0].Predicate);
        Assert.Equal("paper", facts[0].Referents[0]);
        Assert.Equal("rock", facts[0].Referents[1]);
        Assert.Equal("beats", facts[1].Predicate);
        Assert.Equal("rock", facts[1].Referents[0]);
        Assert.Equal("scissors", facts[1].Referents[1]);
    }

    [Fact]
    public void TestMinecraftWeaponsLogic() {
        var parser = new FOLParser();
        var initialFacts = parser.ParseFacts(
            "beats(diamond_sword, diamond_axe); " +
            "beats(stone_pick, stone_shovel); " +
            "beats(diamond_axe, iron_axe); " +
            "beats(iron_axe, stone_shovel); " +
            "beats(iron_pick, stone_pick); " +
            "beats(iron_axe, iron_pick); " +
            "beats(stone_shovel, fist)"
        );
        var kb = new FOLKnowledgeBase(initialFacts);
        var rules = new List<FOLRuleExpression> {
            // beats(?x, ?y) & beats(?y, ?z) _> beats(?x, ?z)
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
        
        // check produced facts
        // 1. diamond_sword beats iron_axe
        Assert.True(kb.Ask(new FOLFact("beats", new[] {
            "diamond_sword", "iron_axe"
        })));
        // 2. diamond_axe beats fist
        Assert.True(kb.Ask(new FOLFact("beats", new[] {
            "diamond_axe", "fist"
        })));
        // 3. iron_pick beats stone_shovel
        Assert.True(kb.Ask(new FOLFact("beats", new[] {
            "iron_pick", "stone_shovel"
        })));
    }
}