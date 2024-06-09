using System.Collections.Generic;
using System.Linq;
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
        var bindings = betterRule.MatchAllPossible(kb);
        Assert.True(bindings.Count > 0);
        var context1 = bindings[0];
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
        var bindings = bothLikePieRule.MatchAllPossible(kb);
        Assert.Equal(1, bindings.Count);
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
        var bindings = personLikesPieAndCake.MatchAllPossible(kb);
        Assert.Equal(1, bindings.Count);
        var context1 = bindings[0];
        Assert.Equal("alice", context1.Get("?person"));
    }

    [Fact]
    public void TestDisjunction1() {
        var kb = new FOLKnowledgeBase(new List<FOLFact> {
            new FOLFact("likes", new[] {
                "alice", "pie"
            }),
        });
        var likesPieOrCake = new FOLRuleExpression(
            new FOLOrExpression(new FOLRuleExpression[] {
                new FOLRule("likes", new[] {
                    "?person", "pie"
                }),
                new FOLRule("likes", new[] {
                    "?person", "cake"
                }),
            })
        );
        var bindings = likesPieOrCake.MatchAllPossible(kb);
        Assert.Equal(1, bindings.Count);
        var context1 = bindings[0];
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
                antecedent: new FOLIfExpression(new FOLRule("likes", new[] {
                    "?person", "dessert"
                })),
                consequent: new FOLThenExpression(new FOLRule("likes", new[] {
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
    public void TestConditionalDelete1() {
        // if alice likes pie, then alice likes cake
        var kb = new FOLKnowledgeBase(new List<FOLFact> {
            new FOLFact("available", new[] {
                "waffles"
            }),
            new FOLFact("today", new[] {
                "monday"
            })
        });
        var rules = new List<FOLRuleExpression> {
            new FOLConditional(
                antecedent: new FOLIfExpression(new FOLRule("today", new[] {
                    "monday"
                })),
                deleter: new FOLDeleteExpression(new FOLRule("available", new[] {
                    "waffles"
                }))
            )
        };
        var prover = new FOLProver();
        prover.ForwardChain(rules, kb);

        // we should have removed available(waffles)
        Assert.False(kb.Ask(new FOLFact("available", new[] {
            "waffles"
        })));
    }

    [Fact]
    public void TestNegation1() {
        var kb = new FOLKnowledgeBase(new List<FOLFact> {
            new FOLFact("boss", new[] {
                "diavolo", "bucciarati"
            }),
            new FOLFact("boss", new[] {
                "diavolo", "polpo"
            }),
            new FOLFact("self", new[] {
                "bucciarati", "bucciarati"
            }),
            new FOLFact("self", new[] {
                "polpo", "polpo"
            }),
        });
        var rules = new List<FOLRuleExpression> {
            // boss(?p, ?x) & boss(?p, ?y) & !self(?x, ?y) _> rival(?x, ?y)
            new FOLConditional(
                antecedent: new FOLIfExpression(new FOLAndExpression(
                    new FOLRuleExpression[] {
                        new FOLRule("boss", new[] {
                            "?p", "?x"
                        }),
                        new FOLRule("boss", new[] {
                            "?p", "?y"
                        }),
                        new FOLNotExpression(new FOLRule("self", new[] {
                            "?x", "?y"
                        }))
                    }
                )),
                consequent: new FOLThenExpression(new FOLRule("rival", new[] {
                    "?x", "?y"
                }))
            )
        };

        var prover = new FOLProver();
        prover.ForwardChain(rules, kb);

        // ensure no self-rival
        Assert.False(kb.Ask(new FOLFact("rival", new[] {
            "bucciarati", "bucciarati"
        })));

        // we should have produced rival(bucciarati, polpo)
        Assert.True(kb.Ask(new FOLFact("rival", new[] {
            "bucciarati", "polpo"
        })));
        // and also rival(polpo, bucciarati)
        Assert.True(kb.Ask(new FOLFact("rival", new[] {
            "polpo", "bucciarati"
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
                antecedent: new FOLIfExpression(new FOLAndExpression(
                    new FOLRuleExpression[] {
                        new FOLRule("greater", new[] {
                            "?x", "?y"
                        }),
                        new FOLRule("greater", new[] {
                            "?y", "?z"
                        })
                    }
                )),
                consequent: new FOLThenExpression(new FOLRuleExpression(
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
        var rb = new FOLLogicBuilder();

        // Initial facts
        var initialFacts = rb.Facts(
            rb.Fact("beats", "diamond_sword", "diamond_axe"),
            rb.Fact("beats", "stone_pick", "stone_shovel"),
            rb.Fact("beats", "diamond_axe", "iron_axe"),
            rb.Fact("beats", "iron_axe", "stone_shovel"),
            rb.Fact("beats", "iron_pick", "stone_pick"),
            rb.Fact("beats", "iron_axe", "iron_pick"),
            rb.Fact("beats", "stone_shovel", "fist")
        );

        var kb = new FOLKnowledgeBase(initialFacts);

        // Rules
        var rules = rb.Rules(
            rb.Cond(
                rb.If(
                    rb.And(
                        rb.Rule("beats", "?x", "?y"),
                        rb.Rule("beats", "?y", "?z")
                    )
                ),
                rb.Then(
                    rb.Rule("beats", "?x", "?z")
                )
            )
        );

        var prover = new FOLProver();
        prover.ForwardChain(rules, kb);

        // Check produced facts
        Assert.True(kb.Ask(rb.Fact("beats", "diamond_sword", "iron_axe")));
        Assert.True(kb.Ask(rb.Fact("beats", "diamond_axe", "fist")));
        Assert.True(kb.Ask(rb.Fact("beats", "iron_pick", "stone_shovel")));
    }

    [Fact]
    public void TestBlackFamilyCousins() {
        var rb = new FOLLogicBuilder();

        // initial facts
        var facts = rb.Facts(
            rb.Fact("person", "sirius"),
            rb.Fact("person", "regulus"),
            rb.Fact("person", "walburga"),
            rb.Fact("person", "alphard"),
            rb.Fact("person", "cygnus"),
            rb.Fact("person", "pollux"),
            rb.Fact("person", "bellatrix"),
            rb.Fact("person", "andromeda"),
            rb.Fact("person", "narcissa"),
            rb.Fact("person", "nymphadora"),
            rb.Fact("person", "draco"),
            rb.Fact("parent", "walburga", "sirius"),
            rb.Fact("parent", "walburga", "regulus"),
            rb.Fact("parent", "pollux", "walburga"),
            rb.Fact("parent", "pollux", "alphard"),
            rb.Fact("parent", "pollux", "cygnus"),
            rb.Fact("parent", "cygnus", "bellatrix"),
            rb.Fact("parent", "cygnus", "andromeda"),
            rb.Fact("parent", "cygnus", "narcissa"),
            rb.Fact("parent", "andromeda", "nymphadora"),
            rb.Fact("parent", "narcissa", "draco")
        );

        // knowledge base
        var kb = new FOLKnowledgeBase(facts);

        // rules
        var rules = rb.Rules(
            // self
            rb.Cond(
                rb.If(rb.Rule("person", "?x")),
                rb.Then(rb.Rule("self", "?x", "?x"))
            ),
            // sibling
            rb.Cond(
                rb.If(
                    rb.And(
                        rb.Rule("parent", "?p", "?x"),
                        rb.Rule("parent", "?p", "?y"),
                        rb.Not(rb.Rule("self", "?x", "?y"))
                    )
                ),
                rb.Then(rb.Rule("sibling", "?x", "?y"))
            ),
            // child
            rb.Cond(
                rb.If(rb.Rule("parent", "?x", "?y")),
                rb.Then(rb.Rule("child", "?y", "?x"))
            ),
            // cousin
            rb.Cond(
                rb.If(
                    rb.And(
                        rb.Rule("parent", "?p", "?x"),
                        rb.Rule("sibling", "?p", "?q"),
                        rb.Rule("parent", "?q", "?y")
                    )
                ),
                rb.Then(rb.Rule("cousin", "?x", "?y"))
            ),
            // grandparent
            rb.Cond(
                rb.If(
                    rb.And(
                        rb.Rule("parent", "?x", "?y"),
                        rb.Rule("parent", "?y", "?z")
                    )
                ),
                rb.Then(rb.Rule("grandparent", "?x", "?z"))
            ),
            // grandchild
            rb.Cond(
                rb.If(
                    rb.And(
                        rb.Rule("parent", "?x", "?y"),
                        rb.Rule("parent", "?y", "?z")
                    )
                ),
                rb.Then(rb.Rule("grandchild", "?z", "?x"))
            )
        );
        
        var prover = new FOLProver();
        prover.ForwardChain(rules, kb);
        
        // we expect exactly 14 cousins
        var cousins = kb.Facts.Where(f => f.Predicate == "cousin").ToList();
        Assert.Equal(14, cousins.Count);
    }
}