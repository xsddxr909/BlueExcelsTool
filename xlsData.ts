
/**
 * 表
 */
export class xlsData
{
    //时段
    public Domain;
    //时段时间（秒）
    public Duration;

    public ID1

    public static Creat(Domain,Duration,ID1):xlsData{
        let data:xlsData=new xlsData();
        data.Domain=Domain;
        data.Duration=Duration;
        data.ID1=ID1;
        return data;
    }
}
export class Config  {
    
    private static m_pInstance: Config;
    public constructor()
    {
    }
    public static Get(): Config
    {
        if(null == Config.m_pInstance)
        {
            Config.m_pInstance = new Config();
        }
        return Config.m_pInstance;
    }

    xlsData:Map<number,xlsData>=new Map();
    
    public init(){
        this.xlsData.set(1,xlsData.Creat(1,2,3));
        this.xlsData.set(1,xlsData.Creat(1,2,3));
        this.xlsData.set(1,xlsData.Creat(1,2,3));
        this.xlsData.set(1,xlsData.Creat(1,2,3));
        this.xlsData.set(1,xlsData.Creat(1,2,3));
       
    }

}