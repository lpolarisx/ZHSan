using System;



namespace GameGlobal
{
    /// <summary>
    /// 玩家可以主动开展的外交方式
    /// </summary>
    public enum DiplomacyKind
    {
        /// <summary>
        /// 亲善
        /// </summary>
        Enhance,

        /// <summary>
        /// 结盟
        /// </summary>
        Ally,

        /// <summary>
        /// 解除同盟
        /// </summary>
        Reset,

        /// <summary>
        /// 声讨
        /// </summary>
        Denounce,

        /// <summary>
        /// 停战
        /// </summary>
        Truce,

        /// <summary>
        /// 劝降
        /// </summary>
        QuanXiang,

        /// <summary>
        /// 谈判：按谈成的条款派使者出发
        /// </summary>
        Negotiate
    }
}
