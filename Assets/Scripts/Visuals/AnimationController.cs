﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;

    public void CrashOpponent()
    {
        m_Animator.SetBool("IsCrashing", true);
    }

    public void BeCrashed()
    {
        m_Animator.SetBool("IsCrashed", true);
    }

    public void ResetAnimation()
    {
        m_Animator.SetBool("IsCrashing", false);
        m_Animator.SetBool("IsCrashed", false);
    }
}