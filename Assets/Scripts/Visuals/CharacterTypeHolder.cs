using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterTypeHolder : MonoBehaviour
{
    [SerializeField] private EPlayerType m_CharacterType;
    public EPlayerType CharacterType => m_CharacterType;
}
