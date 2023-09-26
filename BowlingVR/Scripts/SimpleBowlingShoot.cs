using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BowlingVR.Scripts
{
    /// <summary>
    /// Implémentation simpliste d'un jeu de bowling
    /// </summary>
    public class SimpleBowlingShoot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreboard;
        [SerializeField] private Bowling _bowling;
        [SerializeField] private Vector3 _keelOrigin;
        [SerializeField] private GameObject _bowlingbowl;

        private uint _fallenKeels;
        private uint _score;

        private List<GameObject> _currentGameKeels;

        private void Awake()
        {
            _fallenKeels = 0;
            _score = 0;
            Play();
        }

        /// <summary>
        /// Fonction appelée pour vérifier l'état du jeu (si le jeu est terminé aka toutes les quilles sont tombées, ou non et reset au besoin)
        /// </summary>
        private void _checkForReset()
        {
            if (_fallenKeels != Bowling.KeelsNumber) return;
            _fallenKeels = 0;
        }

        /// <summary>
        /// Met à jour le score
        /// </summary>
        private void _updateScore()
        {
            _fallenKeels += 1;
            _score += 1;
            if (ReferenceEquals(_scoreboard, null)) return;
            _scoreboard.text = _score.ToString();
        }

        /// <summary>
        /// Recommence une partie
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Play()
        {
            SpawnBowl();  // On fait apparaître une première boule
            if (!ReferenceEquals(_currentGameKeels, null) && _currentGameKeels.Count > 0)  // Si jamais il existe déjà des quilles on les supprime
            {
                _currentGameKeels.ForEach(Destroy);
            }
            // Explication du code ci dessous:
            // Utilise des principe de programmation fonctionelle pour récupérer les quilles placées, et pour chacune d'entre elles ajouter les callbacks avant de les mettre en liste et les enregistrer
            // Select est l'équivalent d'une fonction map en js/ts/python etc.
            _currentGameKeels = _bowling.PlaceKeels(_keelOrigin)  // On place les quilles et on ajoute leurs callbacks. 
                .Select(keel =>
                {
                    var keelComponent = keel.GetComponent<Keel>();
                    if (ReferenceEquals(keelComponent, null)) throw new Exception("Badly setup keel");
                    keelComponent += _updateScore;
                    keelComponent += _checkForReset;
                    keelComponent += () =>
                    {
                        StartCoroutine(keelComponent.removeKeelAfter(5));
                    };
                    return keel;
                })
                .ToList();
        }
    
        /// <summary>
        /// Remet les scores à zéro dans l'UI
        /// </summary>
        public void ResetScore()
        {
            _score = 0;
            if (ReferenceEquals(_scoreboard, null)) return;
            _scoreboard.text = _score.ToString();
        }
    
        public void SpawnBowl()
        {
            var currTransform = gameObject.transform;
            var currPosition = currTransform.position;
            var dest = new Vector3(currPosition.x, 1f, currPosition.z);
            var bowl = Instantiate(_bowlingbowl, dest, quaternion.identity, currTransform);
            var collider = bowl.AddComponent<SphereCollider>();
            var bowlRigidBody = bowl.AddComponent<Rigidbody>();
            bowlRigidBody.angularDrag = 0.01f;
            var xrgrabbable = bowl.AddComponent<Draggable>();
            bowlRigidBody.mass = 1f;
        }
    }
}
