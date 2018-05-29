using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JMiles42 {
    public class Helpers {
        public static List<Health> GetTargetsInRange(Health MyHealth, float AttackRange) {
            var list = new List<Health>();
            var targets = Physics.OverlapSphere(MyHealth.transform.position, AttackRange);
            foreach (var target in targets) {
                if (target.GetComponentInParent<Health>()) {
                    if (target.GetComponentInParent<Health>() == MyHealth) continue;
                    list.Add(target.GetComponentInParent<Health>());
                }
            }
            return list;
        }

        public static List<Health> GetTargetsInRange(Health MyHealth, float AttackRange, Health friend) {
            var list = new List<Health>();
            var targets = Physics.OverlapSphere(MyHealth.transform.position, AttackRange);
            foreach (var target in targets) {
                if (target.GetComponent<Health>()) {
                    if (target.GetComponent<Health>() == MyHealth) continue;
                    if (friend == target.GetComponent<Health>()) continue;
                    list.Add(target.GetComponent<Health>());
                    continue;
                }
                else if (target.GetComponentInParent<Health>()) {
                    if (target.GetComponentInParent<Health>() == MyHealth) continue;
                    if (friend == target.GetComponentInParent<Health>()) continue;
                    list.Add(target.GetComponentInParent<Health>());
                    continue;
                }
            }
            return list;
        }

        public static List<Health> GetTargetsInRange(Health MyHealth, float AttackRange, IEnumerable<Health> friends) {
            var list = new List<Health>();
            var targets = Physics.OverlapSphere(MyHealth.transform.position, AttackRange);
            foreach (var target in targets) {
                if (target.GetComponent<Health>()) {
                    if (target.GetComponent<Health>() == MyHealth) continue;
                    if (friends.Contains(target.GetComponent<Health>())) continue;
                    list.Add(target.GetComponent<Health>());
                    continue;
                }
                else if (target.GetComponentInParent<Health>()) {
                    if (target.GetComponentInParent<Health>() == MyHealth) continue;
                    if (friends.Contains(target.GetComponentInParent<Health>())) continue;
                    list.Add(target.GetComponentInParent<Health>());
                    continue;
                }
            }
            return list;
        }


        public static List<CharacterBase> GetTargetsInRange(CharacterBase MyHealth, float AttackRange) {
            var list = new List<CharacterBase>();
            var targets = Physics.OverlapSphere(MyHealth.transform.position, AttackRange);
            foreach (var target in targets) {
                if (target.GetComponentInParent<CharacterBase>()) {
                    if (target.GetComponentInParent<CharacterBase>() == MyHealth) continue;
                    list.Add(target.GetComponentInParent<CharacterBase>());
                }
            }
            return list;
        }

        public static List<CharacterBase> GetTargetsInRange(CharacterBase MyHealth, float AttackRange, CharacterBase friend) {
            var list = new List<CharacterBase>();
            var targets = Physics.OverlapSphere(MyHealth.transform.position, AttackRange);
            foreach (var target in targets) {
                if (target.GetComponent<CharacterBase>()) {
                    if (target.GetComponent<CharacterBase>() == MyHealth) continue;
                    if (friend == target.GetComponent<CharacterBase>()) continue;
                    list.Add(target.GetComponent<CharacterBase>());
                    continue;
                }
                else if (target.GetComponentInParent<CharacterBase>()) {
                    if (target.GetComponentInParent<CharacterBase>() == MyHealth) continue;
                    if (friend == target.GetComponentInParent<CharacterBase>()) continue;
                    list.Add(target.GetComponentInParent<CharacterBase>());
                    continue;
                }
            }
            return list;
        }

        public static List<CharacterBase> GetTargetsInRange(CharacterBase MyHealth, float AttackRange, IEnumerable<CharacterBase> friends) {
            var list = new List<CharacterBase>();
            var targets = Physics.OverlapSphere(MyHealth.transform.position, AttackRange);
            foreach (var target in targets) {
                if (target.GetComponent<CharacterBase>()) {
                    if (target.GetComponent<CharacterBase>() == MyHealth) continue;
                    if (friends.Contains(target.GetComponent<CharacterBase>())) continue;
                    list.Add(target.GetComponent<CharacterBase>());
                    continue;
                }
                else if (target.GetComponentInParent<CharacterBase>()) {
                    if (target.GetComponentInParent<CharacterBase>() == MyHealth) continue;
                    if (friends.Contains(target.GetComponentInParent<CharacterBase>())) continue;
                    list.Add(target.GetComponentInParent<CharacterBase>());
                    continue;
                }
            }
            return list;
        }
    }
}