using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShotSystem
{
    [Serializable]
    public class Pattern
    {
        public List<ShootDef> m_Directions = new List<ShootDef>();
        public int m_RepeatAmount = 1;
        public float m_RepeatInterval = 0;
    }

    [Serializable]
    public class ShootDef
    {
        public LevelManager.Pair<int, int> m_Dir;
        public LevelManager.Pair<float, float> m_Disp;
    }

    /* 
     
    El personaje tiene que tener si es volador o de tierra, que se pasar� al disparo para que sepa si interactuar o no con los obst�culos
    El efecto del proyectil deber�a ser alg�n tipo de funci�n an�nima pasada por par�metro que interactue con el tablero revisando las casillas por las que pasa
    El da�o que hacen los proyectiles deber ser pasado por la entidad, y si deja efectos, estos ser�n proporcionales

     */

    public static IEnumerator ShootPattern(Pattern pattern, GameObject projectile, BoardElement parent, Vector2 shootDir, float distanceToEntity, Transform bulletParent, Action<BoardElement, GameObject, float, Transform, Vector3, Vector2Int> instantiateFunc)
    {
        float angle = Vector2.SignedAngle(Vector2.up, shootDir);

        for(int i = 0; i < pattern.m_RepeatAmount; ++i)
        {
            foreach (var direction in pattern.m_Directions)
            {
                Vector3 rotatedDirection = Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(direction.m_Dir.Key, direction.m_Dir.Value);
                Vector3 rotatedDisplacement = Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(direction.m_Disp.Key, direction.m_Disp.Value, 0);
                instantiateFunc(parent, projectile, distanceToEntity, bulletParent, rotatedDisplacement, new Vector2Int(Mathf.RoundToInt(rotatedDirection.x), Mathf.RoundToInt(rotatedDirection.y)));
            }
            yield return new WaitForSeconds(pattern.m_RepeatInterval);
        }

        yield return null;
    }
}
