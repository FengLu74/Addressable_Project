using UnityEngine;
// ReSharper disable IdentifierTypo
namespace Kit {
    public interface IRandomizable<T> {
        int Rate { get; set; }
        T Object { get; set; }
    }

    public struct Randomizable<T> : IRandomizable<T> {
        private T _object;
        private int _rate;

        public Randomizable(T obj, int rate) {
            _object = obj;
            _rate = rate;
        }

        int IRandomizable<T>.Rate {
            get => _rate;
            set => _rate = value;
        }

        T IRandomizable<T>.Object {
            get => _object;
            set => _object = value;
        }

        public T GetObject() => _object;
    }

    public class Randomizer<T> {
        private IRandomizable<T>[] randomizables;
        private bool calculated;
        private int total;
        private int step;

        public int Count { get; }

        public Randomizer(int count) {
            Count = count;

            step = 0;
            calculated = false;
            total = 0;
            randomizables = new IRandomizable<T>[count];
        }

        private void Calculate() {
            if (calculated) {
                return;
            }

            for (var i = 0; i < Count; i++) {
                total += randomizables[i].Rate;
            }

            calculated = true;
        }

        public void AddMember(T t, int rate) {
            if (step >= Count) {
                Debug.LogError("[Randomizer] Capacity is full.");
                return;
            }

            randomizables[step] = new Randomizable<T>(t, rate);

            step++;
        }

        public T Select() {
            Calculate();

            var randomNumber = Random.Range(0, total);
            var accumulatedProbability = 0;
            for (var i = 0; i < Count; i++) {
                accumulatedProbability += randomizables[i].Rate;
                if (randomNumber <= accumulatedProbability) {
                    return randomizables[i].Object;
                }
            }
            return new Randomizable<T>().GetObject();
        }

        public T Pick(int index) => randomizables[index].Object;
    }
}
