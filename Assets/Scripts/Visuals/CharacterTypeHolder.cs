using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterTypeHolder : MonoBehaviour
{
    [SerializeField] private ECharacterType m_CharacterType;
    public ECharacterType CharacterType => m_CharacterType;
}
