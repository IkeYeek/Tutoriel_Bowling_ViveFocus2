using System;
using System.Collections.Generic;
using UnityEngine;

namespace BowlingVR.Scripts
{
    /// <summary>
    /// Se charge de disposer le niveau
    /// </summary>
    public class Bowling : MonoBehaviour
    {
        public static uint KeelsNumber => _keelsNumber;
        [SerializeField] private static uint _keelsNumber = 10;
        [SerializeField] private GameObject keelPrefab;

        public static Material keelMaterial { get; private set; }

        private void Awake()
        {
            keelMaterial = Resources.Load<Material>("Keel");
        }

        /// <summary>
        /// Place le nombre le plus proche inférieur à n de quille afin d'avoir une disposition de bowling réglementaire
        /// On part du principe que le jeu se fait le long de l'axe Z.
        /// </summary>
        /// <param name="n">Le nombre de quilles que l'on veut (borne haute) on se retrouve avec ce nombre ou le nombre inférieur le plus proche donnant une configuration valide</param>
        /// <param name="firstKeelPosition">La position de la première quille</param>
        /// <returns></returns>
        public IEnumerable<GameObject> PlaceKeels(Vector3 firstKeelPosition)
        {
            const float spaceAround = 0.3048f;  // Espacement entre deux quilles, 30.48cm
            var currKeelPosition = firstKeelPosition;
            var deltaRoot = Math.Sqrt(1 - 4 * (-2 * _keelsNumber));
            var lineCount = Math.Max((-1 - deltaRoot) / 2, (-1 + deltaRoot) / 2);
            for (var i = 0u; i < lineCount; i += 1)
            {
                for (var keelIndex = 0u; keelIndex < i + 1; keelIndex += 1)
                {
                    var keel = Instantiate(keelPrefab, currKeelPosition, Quaternion.identity, transform);
                    keel.transform.Rotate(new Vector3(1, 0, 0), -90);
                    currKeelPosition = new Vector3(currKeelPosition.x + spaceAround, currKeelPosition.y, currKeelPosition.z);
                    keel.AddComponent<Keel>();
                    yield return keel;
                }
                currKeelPosition = new Vector3(currKeelPosition.x - ((i+1)*spaceAround + spaceAround/2f), currKeelPosition.y, currKeelPosition.z + spaceAround);
            }
        }
    }
}