using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress.AB
{
    public enum ABType
    {
        [ABTypeDes("")]
        Null = 0,
        /// <summary>
        /// 精灵
        /// </summary>
        [ABTypeDes("sprite_")]
        Sprite = 1,

        /// <summary>
        /// 纯图片
        /// </summary>
        [ABTypeDes("tex_")]
        Texture,

        /// <summary>
        /// 特效 预制件
        /// </summary>
        [ABTypeDes("eff_")]
        Effect,

        /// <summary>
        /// 窗体 预制件
        /// </summary>
        [ABTypeDes("w_")]
        Window,

        /// <summary>
        /// 其它 预制件
        /// </summary>
        [ABTypeDes("op_")]
        OtherPrefab,

        /// <summary>
        /// 场景（unity3d场景）
        /// </summary>
        [ABTypeDes("s_")]
        Scene,

        /// <summary>
        /// 音效
        /// </summary>
        [ABTypeDes("a_")]
        Audio,

        [ABTypeDes("clip_")]
        AnimationClip,
        /// <summary>
        /// 视频
        /// </summary>
        [ABTypeDes("v_")]
        Video,

        /// <summary>
        /// FBX
        /// </summary>
        [ABTypeDes("fbx_")]
        FBX,

        /// <summary>
        /// 模型
        /// </summary>
        [ABTypeDes("mesh_")]
        Mesh,

        /// <summary>
        /// 材质球
        /// </summary>
        [ABTypeDes("mat_")]
        Material,

        /// <summary>
        /// AnimatorController
        /// </summary>
        [ABTypeDes("ac_")]
        AnimatorController,

        /// <summary>
        /// 玩家自定义的二进制文件
        /// </summary>
        [ABTypeDes("b_")]
        Byte,

        /// <summary>
        /// 通用的继承自UnityEngine.Object的
        /// </summary>
        [ABTypeDes("o_")]
        Object,
    }
}