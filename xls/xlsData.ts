
/**
 * 表
 */
export class xlsData
{
    //时段
    public Domain:number;
    //时段时间（秒）
    public Duration:string;

    public ID1:boolean;

    public nstr:string[];

    public nnum:number[];

    public static Creat(Domain:number,Duration:string,ID1:boolean,nstr:string[],nnum:number[]):xlsData{
        let data:xlsData=new xlsData();
        data.Domain=Domain;
        data.Duration=Duration;
        data.ID1=ID1;
        data.nstr=nstr;
        data.nnum=nnum;
        return data;
    }
}
export class Config  {
    
    private static m_pInstance: Config;
    public static Get(): Config{ 
        if(null == Config.m_pInstance){ Config.m_pInstance = new Config(); Config.m_pInstance.init(); } return Config.m_pInstance; }
    private inited:boolean=false;
    //多表; str2
    xlsData:Map<number,xlsData>=new Map<number,xlsData>();
   

    
    public init(){
        if(this.inited)return;
        
        //str3
        this.xlsData.set(1,xlsData.Creat(1,"2",true,["1","1","2","3"],[1,1,2,3]));
        this.xlsData.set(2,xlsData.Creat(2,"2",true,["1","1","2","3"],[1,1,2,3]));
        this.xlsData.set(3,xlsData.Creat(3,"2",true,["1","1","2","3"],[1,1,2,3]));
       
        this.inited=true;
    }

}