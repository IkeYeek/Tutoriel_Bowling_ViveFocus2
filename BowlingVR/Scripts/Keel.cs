using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace BowlingVR.Scripts
{
    /// <summary>
    /// Implémentation d'une quille capable de déclancher des callbacks quand elle tombe
    /// </summary>
    public class Keel : MonoBehaviour
    {
        private List<Action> _callbacks;
        private Quaternion _originalAngle;
        private bool _fallen;
        private bool _notified;
    
        public IEnumerator removeKeelAfter(uint time)
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }
        /// <summary>
        /// Ajoute une action (une fonction qui ne retourne rien) à la liste des actions à appeler. Toutes les actions sont appelées lorsque la quille tombe
        /// </summary>
        /// <param name="a"></param>
        public void AddCallback(Action a)
        {
            _callbacks.Add(a);
        }

        /// <summary>
        ///  [Optionnel] Cette surcharge d'opérateur sert à rendre notre code plus intelligible. Faire "quille += Action" ajoutera l'action à la liste des callbacks de la quille
        /// </summary>
        /// <param name="k"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Keel operator +(Keel k, Action a)
        {
            k.AddCallback(a);
            return k;
        }
    
        private void Awake()
        {
            _callbacks = new List<Action>();  // On initialise la liste des actions à appeler lorsque la quille tombe
            _fallen = false; 
            _notified = false;
            _originalAngle = transform.rotation;
            var keelCollider = gameObject.AddComponent<MeshCollider>();  // On ajoute un collider à notre quille afin de lui permettre d'intéragir avec d'autres GameObject disposant d'un collider 
            keelCollider.convex = true;
            var rb = gameObject.AddComponent<Rigidbody>();  // On lui ajoute un rigidbody, permettant d'activer la gravité et de se comporter comme un objet physique
            rb.mass = .25f;
        }

        private void _callCallbacks()
        {
            _callbacks.ForEach(cb => cb.Invoke());
        }

        private void LateUpdate()
        {
            if (_notified) return;  // Si jamais on a déjà appelé les callbacks, il n'y a plus rien à faire
            if (_fallen)  // Si la quille vient de tomber mais que l'on a pas appelé les callbacks, alors on le fait maintenant
            {
                _callCallbacks();
                _notified = true;
                return;
            }

            var angle = Quaternion.Angle(_originalAngle, transform.rotation);
            if (angle > 45) _fallen = true;  // Si la quille affiche un angle > a 45°, elle est considéree comme tombée.
            // [Optionnel] Rajouter une vérification de l'emplacement de la quille afin de la compter comme tombée si elle s'est suffisament déplacée
        }
    }
}
