using RogalTutorial.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogalTutorial.Systems
{
    /// <summary>
    /// Kolejność poruszania się różnych aktorów
    /// </summary>
    public class SchedulingSystem
    {
        /// <summary>
        /// tura
        /// </summary>
        private int _time;

        /// <summary>
        /// Słownik który 
        /// </summary>
        private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;

        public SchedulingSystem()
        {
            _time = 0;
            _scheduleables = new SortedDictionary<int, List<IScheduleable>>();
        }
        
        /// <summary>
        /// Dodaj nowy obiekt do harmonogramu
        /// </summary>
        /// <param name="scheduleable"></param>
        public void Add(IScheduleable scheduleable)
        {
            // Ustaw czas dla obiektu jako klucz słownika
            int key = _time + scheduleable.Time;
            if (!_scheduleables.ContainsKey(key))
                _scheduleables.Add(key, new List<IScheduleable>());

            _scheduleables[key].Add(scheduleable);
        }

        /// <summary>
        /// Usuń obiekt z harmonogramu
        /// </summary>
        /// <param name="scheduleable"></param>
        public void Remove(IScheduleable scheduleable)
        {
            KeyValuePair<int, List<IScheduleable>> scheduleableListFound
              = new KeyValuePair<int, List<IScheduleable>>(-1, null);

            foreach (var scheduleablesList in _scheduleables)
            {
                if (scheduleablesList.Value.Contains(scheduleable))
                {
                    scheduleableListFound = scheduleablesList;
                    break;
                }
            }
            if (scheduleableListFound.Value != null)
            {
                scheduleableListFound.Value.Remove(scheduleable);
                if (scheduleableListFound.Value.Count <= 0)
                    _scheduleables.Remove(scheduleableListFound.Key);
            }
        }

        /// <summary>
        /// Pobierz hanmonogram
        /// </summary>
        /// <returns></returns>
        public IScheduleable Get()
        {
            var firstScheduleableGroup = _scheduleables.First();
            var firstScheduleable = firstScheduleableGroup.Value.First();
            Remove(firstScheduleable);
            _time = firstScheduleableGroup.Key;
            return firstScheduleable;
        }

        /// <summary>
        /// Pobierz aktualny czas dla harmonogramu
        /// </summary>
        /// <returns></returns>
        public int GetTime()
        {
            return _time;
        }

        /// <summary>
        /// Resetuj czas oraz wyczyść listę harmonogram
        /// </summary>
        public void Clear()
        {
            _time = 0;
            _scheduleables.Clear();
        }
    }
}
