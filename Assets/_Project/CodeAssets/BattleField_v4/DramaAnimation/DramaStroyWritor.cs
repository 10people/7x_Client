#define DEBUG_DRAMA_STORY_WRITER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

public class DramaStroyWritor : MonoBehaviour
{
	private DramaStoryControllor controllor;


	public void checkout()
	{
		controllor = (DramaStoryControllor)gameObject.GetComponentInChildren (typeof(DramaStoryControllor));

		DramaStoryBoard[] dsbs = controllor.GetComponentsInChildren<DramaStoryBoard> ();

		foreach(DramaStoryBoard dsb in dsbs)
		{
			controllor.storyBoardList.Add (dsb.storyBoardId, dsb);
		}

		controllor.init ();

		controllor.lightOn ();

		foreach(DramaStoryBoard dsb in controllor.storyBoardList.Values)
		{
			dsb.initWithWritor();

			writeXml(dsb);
		}
	}

	private void writeXml(DramaStoryBoard dsb)
	{
		StreamWriter sw;

		string t_file_path = Application.dataPath + "/Resources/_Data/BattleField/StoryBoard/" + "storyBoard_" + dsb.storyBoardId + ".xml";

		{
			#if DEBUG_DRAMA_STORY_WRITER
			Debug.Log( "DramaStoryReander.Write( " + t_file_path + " )" );
			#endif
		}

		FileInfo t = new FileInfo( t_file_path );

		sw = t.CreateText();
		JSONClass json = new JSONClass ();

		json ["storyboardId"].AsInt = dsb.storyBoardId;

		foreach(DramaActorControllor actorControllor in dsb.getDramaActorControllors())
		{
			JSONClass jsonActor = new JSONClass();

			jsonActor["actorModelId"].AsInt = actorControllor.actorModelId;

			jsonActor["actorFlagId"].AsInt = actorControllor.actorFlagId;

			jsonActor["actorName"] = actorControllor.actorName;

			jsonActor["actorJuqing"].AsBool = actorControllor.m_isJuqing;

			jsonActor["startPositionx"].AsFloat = actorControllor.startPosition.x;

			jsonActor["startPositiony"].AsFloat = actorControllor.startPosition.y;

			jsonActor["startPositionz"].AsFloat = actorControllor.startPosition.z;

			jsonActor["startRotationx"].AsFloat = actorControllor.startRotation.x;

			jsonActor["startRotationy"].AsFloat = actorControllor.startRotation.y;

			jsonActor["startRotationz"].AsFloat = actorControllor.startRotation.z;

			jsonActor["startScale"].AsFloat = actorControllor.startScale;

			Component[] coms = actorControllor.gameObject.GetComponents(typeof(DramaActor));

			foreach(Component com in coms)
			{
				JSONClass node = new JSONClass();
				
				DramaActor da = (DramaActor)com;

				node["waittingTime"].AsFloat = da.waittingTime;

				node["actorType"].AsInt = (int)da.actorType;

				if(da.actorType == DramaActor.ACTOR_TYPE.ANIM)
				{
					DramaActorPlayAnim dapa = (DramaActorPlayAnim)da;

					node["animName"] = dapa.animName;

					node["playTime"].AsFloat = dapa.playTime;

					node["isStand"].AsBool = dapa.m_isStand;
				}
				else if(da.actorType == DramaActor.ACTOR_TYPE.APPEAR)
				{
					DramaActorAppear daa = (DramaActorAppear)da;

					node["appear"].AsBool = daa.appear;
				}
				else if(da.actorType == DramaActor.ACTOR_TYPE.CAMERA_ANIMATION)
				{
					DramaActorCameraMove dacm = (DramaActorCameraMove)da;

					node["movingTime"].AsFloat = dacm.movingTime;

					node["CaneraAnimatorId"] = dacm.m_AnimationPath;

					node["parentNodeId"].AsInt = dacm.parentNodeId;

					jsonActor["actorModelId"].AsInt = -1;
				}
				else if(da.actorType == DramaActor.ACTOR_TYPE.CAMERA_ROTATE)
				{
					DramaActorCameraRotate dacr = (DramaActorCameraRotate)da;

					node["targetRotationx"].AsFloat = dacr.targetRotation.x;

					node["targetRotationy"].AsFloat = dacr.targetRotation.y;

					node["targetRotationz"].AsFloat = dacr.targetRotation.z;

					node["rotateTime"].AsFloat = dacr.rotateTime;

					jsonActor["actorModelId"].AsInt = -1;
				}
				else if(da.actorType == DramaActor.ACTOR_TYPE.MOVE)
				{
					DramaActorMove dam = (DramaActorMove)da;

					node["targetPositionx"].AsFloat = dam.targetPosition.x;

					node["targetPositiony"].AsFloat = dam.targetPosition.y;

					node["targetPositionz"].AsFloat = dam.targetPosition.z;

					node["movingTime"].AsFloat = dam.movingTime;
				}
				else if(da.actorType == DramaActor.ACTOR_TYPE.ALPHABG)
				{
					DramaActorAlpha dam = (DramaActorAlpha)da;
					
					node["needTime"].AsFloat = dam.m_fNeedTime;
					
					node["endAlpha"].AsFloat = dam.m_fEndAlpha;

					jsonActor["actorModelId"].AsInt = -1;
				}
				else if(da.actorType == DramaActor.ACTOR_TYPE.EFFECT)
				{
					DramaActorPlayEffect dape = (DramaActorPlayEffect)da;

					node["effectid"].AsInt = dape.effectid;

					node["playTime"].AsFloat = dape.playTime;

					node["follow"].AsBool = dape.follow;

					node["px"].AsFloat = dape.position.x;

					node["py"].AsFloat = dape.position.y;

					node["pz"].AsFloat = dape.position.z;

					node["fx"].AsFloat = dape.foward.x;
					
					node["fy"].AsFloat = dape.foward.y;
					
					node["fz"].AsFloat = dape.foward.z;
				}
				else if(da.actorType == DramaActor.ACTOR_TYPE.SOUND)
				{
					DramaActorPlaySound daps = (DramaActorPlaySound)da;

					node["soundId"].AsInt = daps.soundId;
				}

				jsonActor["actors"].AsArray.Add(node);
			}

			json["jsonActor"].AsArray.Add(jsonActor);
		}

		string jsonStr = json.ToString();

		Debug.Log ("writeout: " + jsonStr);
		
		sw.WriteLine(jsonStr);
		
		sw.Close();
		
		sw.Dispose();
	}

}
