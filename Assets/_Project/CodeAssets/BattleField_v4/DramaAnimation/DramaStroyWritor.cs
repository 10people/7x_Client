#define DEBUG_DRAMA_STORY_WRITER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

public class DramaStroyWritor : MonoBehaviour
{
	private DramaStoryControllor controllor;

	void OnDestroy(){
		controllor = null;
	}

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

		json ["totalTime"].AsFloat = dsb.totalTime;

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
				else if(da.actorType == DramaActor.ACTOR_TYPE.ROTATE)
				{
					DramaActorRotate dar = (DramaActorRotate)da;

					node["targetRotationx"].AsFloat = dar.targetRotation.x;
					
					node["targetRotationy"].AsFloat = dar.targetRotation.y;
					
					node["targetRotationz"].AsFloat = dar.targetRotation.z;
					
					node["rotateTime"].AsFloat = dar.rotateTime;
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

					node["moveTime"].AsFloat = dape.moveTime;

					node["follow"].AsBool = dape.follow;

					node["px"].AsFloat = dape.position.x;

					node["py"].AsFloat = dape.position.y;

					node["pz"].AsFloat = dape.position.z;

					node["fx"].AsFloat = dape.foward.x;
					
					node["fy"].AsFloat = dape.foward.y;
					
					node["fz"].AsFloat = dape.foward.z;

					node["tx"].AsFloat = dape.targetLocalPosition.x;
					
					node["ty"].AsFloat = dape.targetLocalPosition.y;
					
					node["tz"].AsFloat = dape.targetLocalPosition.z;
				}
				else if(da.actorType == DramaActor.ACTOR_TYPE.SOUND)
				{
					DramaActorPlaySound daps = (DramaActorPlaySound)da;

					node["soundId"].AsInt = daps.soundId;
				}
				else if(da.actorType == DramaActor.ACTOR_TYPE.UISprite)
				{
					DramaActorSprite das = (DramaActorSprite)da;

					das.refreshDatasWhenCheckout();

					node["anchor"].AsInt = (int)das.anchor;

					node["spriteName"] = das.spriteName;

					node["localPositionx"].AsFloat = das.localPosition.x;

					node["localPositiony"].AsFloat = das.localPosition.y;

					node["localPositionz"].AsFloat = das.localPosition.z;

					node["localRotationx"].AsFloat = das.localRotation.x;
					
					node["localRotationy"].AsFloat = das.localRotation.y;
					
					node["localRotationz"].AsFloat = das.localRotation.z;

					node["dimensionsx"].AsFloat = das.dimensions.x;

					node["dimensionsy"].AsFloat = das.dimensions.y;

					node["depth"].AsInt = das.depth;

					foreach(DramaActorUIData data in das.datas)
					{
						node["datas"].AsArray.Add( data.getJson());
					}
				}
				else if(da.actorType == DramaActor.ACTOR_TYPE.UILabel)
				{
					DramaActorLabel dal = (DramaActorLabel)da;
					
					dal.refreshDatasWhenCheckout();
					
					node["anchor"].AsInt = (int)dal.anchor;
					
					node["text"] = dal.text;
					
					node["localPositionx"].AsFloat = dal.localPosition.x;
					
					node["localPositiony"].AsFloat = dal.localPosition.y;
					
					node["localPositionz"].AsFloat = dal.localPosition.z;
					
					node["localRotationx"].AsFloat = dal.localRotation.x;
					
					node["localRotationy"].AsFloat = dal.localRotation.y;
					
					node["localRotationz"].AsFloat = dal.localRotation.z;

					node["fontSize"].AsInt = dal.fontSize;

					node["fontStyle"].AsInt = (int)dal.fontStyle;

					node["applyGradient"].AsBool = dal.applyGradient;

					node["gradientTopx"].AsFloat = dal.gradientTop.r;

					node["gradientTopy"].AsFloat = dal.gradientTop.g;

					node["gradientTopz"].AsFloat = dal.gradientTop.b;

					node["gradientTopw"].AsFloat = dal.gradientTop.a;

					node["gradientBottomx"].AsFloat = dal.gradientBottom.r;
					
					node["gradientBottomy"].AsFloat = dal.gradientBottom.g;

					node["gradientBottomz"].AsFloat = dal.gradientBottom.b;

					node["gradientBottomw"].AsFloat = dal.gradientBottom.a;
					
					node["labelEffect"].AsInt = (int)dal.labelEffect;

					node["effectColorx"].AsFloat = dal.effectColor.r;

					node["effectColory"].AsFloat = dal.effectColor.g;
					
					node["effectColorz"].AsFloat = dal.effectColor.b;
					
					node["effectColorw"].AsFloat = dal.effectColor.a;
					
					node["effectDistancex"].AsFloat = dal.effectDistance.x;
					
					node["effectDistancey"].AsFloat = dal.effectDistance.y;

					node["labelColorx"].AsFloat = dal.labelColor.r;
					
					node["labelColory"].AsFloat = dal.labelColor.g;

					node["labelColorz"].AsFloat = dal.labelColor.b;
					
					node["labelColorw"].AsFloat = dal.labelColor.a;

					node["dimensionsx"].AsFloat = dal.dimensions.x;
					
					node["dimensionsy"].AsFloat = dal.dimensions.y;
					
					node["depth"].AsInt = dal.depth;
					
					foreach(DramaActorUIData data in dal.datas)
					{
						node["datas"].AsArray.Add( data.getJson());
					}
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
