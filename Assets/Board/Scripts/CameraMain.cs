using UnityEngine;
using System.Collections;

public class CameraMain : MonoBehaviour {
	
	float SLOW_FACTOR_ROTATE=0.95F;
	float SLOW_FACTOR_ZOOM=0.95F;
	
	
	private Vector3 vecCameraAim=new Vector3(0.0995F, 7.4201F,  -0.096F);
	private Vector3 UP_AXIS=new Vector3(0.0f,1.0f,0.0f);
	private float angleVert;
	private float zoomSum;
	
	private bool orbitHorMinus=false;
	private bool orbitHorPlus=false;
	
	private bool orbitVerMinus=false;
	private bool orbitVerPlus=false;
	
	private bool zoomIn=false;
	private bool zoomOut=false;
	
	private float rotationHorSpeed;
	private float rotationVerSpeed;
	private float zoomSpeed;

	private const float NORMAL_PARAM=9.0F;
	
	private const float ZOOM_SUM_MAX=0.8f;
	private const float ZOOM_SUM_MIN=0.0f;
	
	private const float ANGLE_VERT_MAX=20.0f;
	private const float ANGLE_VERT_MIN=-10.0f;
	
	private const float TIME_DELTA_ACTION=0.1F;
	private float blockTime=0.0F;
	
	void Start () {
		angleVert=0.0f;
		
	
	}
	
	void FixedUpdate () {
		Touch[] touches=Input.touches;
		
		
		
		
		if(touches.Length==1 && blockTime<Time.time){
			Vector2 deltaPos=touches[0].deltaPosition;
			/*
			GameObject go=GameObject.Find("TEST");
			GUIText trsf=(GUIText) go.GetComponent("GUIText");
			*/
			//trsf.text=deltaPos.y.ToString();
			if(Mathf.Abs(deltaPos.y)>0.2F){
				rotationVerSpeed=(-deltaPos.y*0.5F*0.7F+ 0.3F*rotationVerSpeed)/2.0F;
				rotateCameraVertical(rotationVerSpeed);
			}
		
			if(Mathf.Abs(deltaPos.x)>0.2F){
					rotationHorSpeed=(deltaPos.x*0.5F*0.7F+	0.3F*rotationHorSpeed)/2.0F;
					rotateCameraHorizontal(rotationHorSpeed);
				
			}
		
			
		}else if(touches.Length>=2){
			blockTime=Time.time+TIME_DELTA_ACTION;
			/*
			GameObject go=GameObject.Find("TESTZoom");
			GUIText trsf=(GUIText) go.GetComponent("GUIText");
			*/
			Vector2 v0=touches[0].position;
			Vector2 v0Delta=touches[0].deltaPosition;
			Vector2 v1=touches[1].position;
			Vector2 v1Delta=touches[1].deltaPosition;
			
			float newDistance=Vector2.Distance(v0+v0Delta, v1+v1Delta);
			float oldDistance=Vector2.Distance(v0,v1);
			
			float factor=newDistance/oldDistance;
			
			float sensitivityFactor=0.002F;
		
			if(Mathf.Abs(factor-1.0F)>sensitivityFactor){
	
				zoomSpeed=((factor-1.0F)*0.7F+0.3F*zoomSpeed)/2.0F;
				zoomCamera(zoomSpeed);
			}
			
			//trsf.text=factor.ToString();
		
		}else{
			
			
			float minValue=0.05F;
			float minValueZoom=0.002F;
			rotationHorSpeed*=SLOW_FACTOR_ROTATE;
			rotationVerSpeed*=SLOW_FACTOR_ROTATE;
			
			zoomSpeed*=SLOW_FACTOR_ZOOM;
			
			if(Mathf.Abs(rotationHorSpeed)>minValue){
			rotateCameraHorizontal(rotationHorSpeed);
			}
			if(Mathf.Abs(rotationVerSpeed)>minValue){
				rotateCameraVertical(rotationVerSpeed);
			}if(Mathf.Abs(zoomSpeed)>minValueZoom){
				zoomCamera(zoomSpeed);
			}
		}
		
		/*
		if(Input.GetMouseButtonDown(0)){
		
		
		}
		else if(Input.GetMouseButtonDown(1)){
			
		}*/
		
		if (Input.GetKeyDown(KeyCode.W)){
			orbitVerPlus=true;
		}
		if (Input.GetKeyUp(KeyCode.W)){
			orbitVerPlus=false;
		}
		 if (Input.GetKeyDown(KeyCode.S)){
			orbitVerMinus=true;
		}
		if (Input.GetKeyUp(KeyCode.S)){
			orbitVerMinus=false;
		}
		 if (Input.GetKeyDown(KeyCode.A)){
			orbitHorPlus=true;
		}
		 if (Input.GetKeyUp(KeyCode.A)){
			orbitHorPlus=false;
		}
		 if (Input.GetKeyDown(KeyCode.D)){
			orbitHorMinus=true;
		}
		 if (Input.GetKeyUp(KeyCode.D)){
			orbitHorMinus=false;
		}
		 if (Input.GetKeyDown(KeyCode.Q)){
			zoomIn=true;
		}
		if (Input.GetKeyUp(KeyCode.Q)){
			zoomIn=false;
		}
		if (Input.GetKeyDown(KeyCode.Z)){
			zoomOut=true;
		}
		 if (Input.GetKeyUp(KeyCode.Z)){
			zoomOut=false;
		}
		
		if(orbitVerPlus){
			rotateCameraVertical(1.0F);
		}
		if(orbitVerMinus){
			rotateCameraVertical(-1.0F);
		}
		if(orbitHorPlus){
			rotateCameraHorizontal(1.0F);
		}
		 if(orbitHorMinus){
			rotateCameraHorizontal(-1.0F);
		}
		if(zoomIn){
			zoomCamera(0.01F);
		}
		if(zoomOut){
			zoomCamera(-0.01F);
		}
	
	}
	private void rotateCameraHorizontal(float angle){
		Vector3 tempV=transform.position;
		transform.position=Quaternion.AngleAxis(angle,UP_AXIS)* tempV;
		transform.forward=vecCameraAim-transform.position;
		/*
		  Vector3 targetPoint = targetPosition.transform.position;
          Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0);    
    */
	}
	private void rotateCameraVertical(float angle){
		
		//if(angleVert<=ANGLE_VERT_MAX && angleVert>=ANGLE_VERT_MIN){
		if(angleVert+angle>ANGLE_VERT_MAX){
			angle=ANGLE_VERT_MAX-angleVert;
		}else if(angleVert+angle<ANGLE_VERT_MIN){
			angle=ANGLE_VERT_MIN-angleVert;
		}
			angleVert+=angle;
			Vector3 tempV=transform.position;
			Vector3 cross=Vector3.Cross(UP_AXIS,transform.forward).normalized;
			transform.position=vecCameraAim+Quaternion.AngleAxis(angle,cross)* (tempV-vecCameraAim);
			transform.forward=vecCameraAim-transform.position;
		//}
		//	Debug.Log("angleVert:"+angleVert);
	}
	private void zoomCamera(float zoomPercent){//100-where object which looks at  Percent of distance
		if(zoomSum+zoomPercent<=ZOOM_SUM_MAX && zoomSum+zoomPercent>=ZOOM_SUM_MIN){
			Vector3 tempV=transform.position;
			zoomSum+=zoomPercent;
			//Debug.Log("ZoomSum:"+zoomSum);
			///Debug.Log("dystans:"+(tempV-vecCameraAim).magnitude);
			transform.position=(tempV-vecCameraAim).normalized*NORMAL_PARAM*(1.0F-zoomSum) +vecCameraAim;
		}
	}

}
