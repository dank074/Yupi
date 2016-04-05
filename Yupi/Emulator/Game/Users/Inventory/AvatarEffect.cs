namespace Yupi.Emulator.Game.Users.Inventory
{
    /// <summary>
    ///     Class AvatarEffect.
    /// </summary>
     class AvatarEffect
    {
        /// <summary>
        ///     The activated
        /// </summary>
         bool Activated;

        /// <summary>
        ///     The effect identifier
        /// </summary>
         int EffectId;

        /// <summary>
        ///     The stamp activated
        /// </summary>
         double StampActivated;

        /// <summary>
        ///     The total duration
        /// </summary>
         int TotalDuration;

        /// <summary>
        ///     The type
        /// </summary>
         short Type;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarEffect" /> class.
        /// </summary>
        /// <param name="effectId">The effect identifier.</param>
        /// <param name="totalDuration">The total duration.</param>
        /// <param name="activated">if set to <c>true</c> [activated].</param>
        /// <param name="activateTimestamp">The activate timestamp.</param>
        /// <param name="type">The type.</param>
         AvatarEffect(int effectId, int totalDuration, bool activated, double activateTimestamp, short type)
        {
            EffectId = effectId;
            TotalDuration = totalDuration;
            Activated = activated;
            StampActivated = activateTimestamp;
            Type = type;
        }

        /// <summary>
        ///     Gets the time left.
        /// </summary>
        /// <value>The time left.</value>
         int TimeLeft
        {
            get
            {
                if (!Activated || TotalDuration == -1)
                    return -1;

                double num = Yupi.GetUnixTimeStamp() - StampActivated;

                if (num >= TotalDuration)
                    return 0;

                return (int) (TotalDuration - num);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has expired.
        /// </summary>
        /// <value><c>true</c> if this instance has expired; otherwise, <c>false</c>.</value>
         bool HasExpired => TimeLeft != -1 && TimeLeft <= 0;

        /// <summary>
        ///     Activates this instance.
        /// </summary>
         void Activate()
        {
            Activated = true;
            StampActivated = Yupi.GetUnixTimeStamp();
        }
    }
}