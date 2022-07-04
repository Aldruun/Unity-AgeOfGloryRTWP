//using UnityEngine;

//public class StatusEffect_StatBuff : StatusEffect {

//    public bool permanent;

//    public int pAttack;
//    public int mAttack;
//    public int pDefence;
//    public int mDefence;

//    public Actor target;

//    public StatusEffect_StatBuff(Actor target, int pAttack, int mAttack, int pDefence, int mDefence, float duration) /*: base(self, interval, duration)*/ {

//        this.target = target;
//        //target.Execute_ModifyStats(pAttack, mAttack, pDefence, mDefence);

//        this.duration = duration;
//        this.permanent = duration < 0;
//    }

//    public override bool Done(Actor agent) {

//        if(agent == null) {

//            return true;
//        }

//        duration -= Time.deltaTime;

//        if(permanent == false && duration <= 0) {

//            //target.Execute_ModifyStats(-pAttack, -mAttack, -pDefence, -mDefence);
//            return true;
//        }

//        return false;
//    }
//}
