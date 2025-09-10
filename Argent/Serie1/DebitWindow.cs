using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argent.Serie1
{
    sealed class DebitWindow
    {
        private static readonly TimeSpan Window = TimeSpan.FromDays(10);
        private readonly List<(DateTime when, decimal amount)> _ops = new();
        private int _start = 0;        // premier élément encore dans la fenêtre
        private decimal _sum = 0m;     // somme des débits dans la fenêtre

        public bool CanDebit(DateTime now, decimal amount, int plafond)
        {
            Prune(now);
            return _sum + amount <= plafond;
        }

        public void Record(DateTime now, decimal amount)
        {
            _ops.Add((now, amount));   // on enregistre uniquement les débits ACCEPTÉS
            _sum += amount;
        }

        private void Prune(DateTime now)
        {
            var min = now - Window;    // fenêtre glissante de 10 jours
            while (_start < _ops.Count && _ops[_start].when <= min)
            {
                _sum -= _ops[_start].amount;
                _start++;
            }
            // Compactage occasionnel pour limiter la mémoire
            if (_start > 256 && _start > _ops.Count / 2)
            {
                _ops.RemoveRange(0, _start);
                _start = 0;
            }
        }
    }
}
