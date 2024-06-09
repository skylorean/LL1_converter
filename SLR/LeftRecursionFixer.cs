using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLRConverter
{
    public class LeftRecursionFixer(List<GrammarRule> grammarRules)
    {
        private const string EMPTY_SYMBOL = "e";
        private readonly List<GrammarRule> _grammarRulesWithoutLeftRecursion = new(grammarRules);

        public List<GrammarRule> GetGrammarRules()
        {
            return _grammarRulesWithoutLeftRecursion;
        }

        public void RemoveLeftRecursion()
        {
            List<GrammarRule> rulesWithLeftRecursion = grammarRules.FindAll(HasLeftRecursion);
            List<GrammarRule> rulesPassed = [];
            foreach (GrammarRule grammarRule in rulesWithLeftRecursion)
            {
                RemoveLeftRecursionForRule(grammarRule, rulesPassed);
                rulesPassed.Add(grammarRule);
            }
        }

        private void RemoveLeftRecursionForRule(GrammarRule rule, List<GrammarRule> rulesPassed)
        {
            // Проверяем, есть ли левая рекурсия в правиле
            if (HasLeftRecursion(rule))
            {
                // Создаем новый нетерминал для замены леворекурсивных правил
                string newToken = rule.Token + "'";

                var rules = _grammarRulesWithoutLeftRecursion.FindAll(x => x.Token == rule.Token && !HasLeftRecursion(x));

                if (rules.Count == 0)
                {
                    throw new Exception("Can't remove left recursion");
                }

                /*B' -> aB'*/
                GrammarRule newRuleForRemoveLeftRecursion = new(newToken, new(rule.SymbolsChain.GetRange(1, rule.SymbolsChain.Count - 1)), new(rule.DirectionSymbols));
                newRuleForRemoveLeftRecursion.SymbolsChain.Add(newToken);

                _grammarRulesWithoutLeftRecursion[_grammarRulesWithoutLeftRecursion.IndexOf(rule)] = newRuleForRemoveLeftRecursion;

                if (rulesPassed.FindAll(x => x.Token == rule.Token).Count > 0)
                {
                    return;
                }

                GrammarRule ruleWithoutLeftRecursion = new(rules[0].Token, [], new(rule.DirectionSymbols));
                for (int i = 0; i < rules.Count; i++)
                {
                    ruleWithoutLeftRecursion = rules[i];

                    if (ruleWithoutLeftRecursion.SymbolsChain.Count == 0)
                    {
                        continue;
                    }

                    GrammarRule newRule;
                    if (ruleWithoutLeftRecursion.SymbolsChain[0] == EMPTY_SYMBOL)
                    {
                        newRule = new(rule.Token, [], new(rule.DirectionSymbols));
                        newRule.SymbolsChain.AddRange(newRuleForRemoveLeftRecursion.SymbolsChain);
                        //newRule.SymbolsChain.Add(newToken);

                        if (rules.FindAll(x => x.SymbolsChain[0] != EMPTY_SYMBOL).Count == 0)
                        {
                            _grammarRulesWithoutLeftRecursion.Insert(_grammarRulesWithoutLeftRecursion.IndexOf(ruleWithoutLeftRecursion) + 1, newRule);
                        }

                        continue;
                    }

                    newRule = new(ruleWithoutLeftRecursion.Token, new(ruleWithoutLeftRecursion.SymbolsChain), new(rule.DirectionSymbols));
                    newRule.SymbolsChain.Add(newToken);

                    _grammarRulesWithoutLeftRecursion[_grammarRulesWithoutLeftRecursion.IndexOf(ruleWithoutLeftRecursion)] = newRule;
                }

                // Добавляем правила для обработки случая epsilon-продукции
                GrammarRule epsilonRule = new(newToken, ["e"], new(rule.DirectionSymbols));

                _grammarRulesWithoutLeftRecursion.Insert(
                    _grammarRulesWithoutLeftRecursion.IndexOf(
                        _grammarRulesWithoutLeftRecursion.FindLast(x => x.Token == newToken)
                    ) + 1,
                    epsilonRule
                );
            }
        }

        private static bool HasLeftRecursion(GrammarRule rule)
        {
            return rule.SymbolsChain.Count > 0 && rule.SymbolsChain[0] == rule.Token;
        }
    }
}