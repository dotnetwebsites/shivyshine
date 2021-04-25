public enum MonthsEnum
{
    NotSet = 0,
    January = 1,
    February = 2,
    March = 3,
    April = 4,
    May = 5,
    June = 6,
    July = 7,
    August = 8,
    September = 9,
    October = 10,
    November = 11,
    December = 12
}

public enum Mail
{
    DNR
}

public enum Documents
{
    APPOINMENTLETTER = 1,
    OFFERLETTER,
    PANCARD,
    AADHAARCARD,
    HIGHESTQUALIFICATION
}

public enum RepositoryResult
{
    Succeeded = 0,
    Failed = 1
}

public enum TokenType
{
    PHONE = 0,
    EMAIL = 1
}

public enum TokenFilter
{
    ID,
    USERID,
    TYPE,
    EMAILPHONE,
    TOKEN,
    ISEXPIRED,
    EXPIREDON,
    TIMESTAMP
}

public enum AssortType
{
    NEW = 0,
    RETURN = 1,
    DAMAGE = 2,
    EXCHANGE = 3,
    WARRENTY_RETURN = 4,
    WARRENTY_EXCHANGE = 5,
    OTHER = 6
}

public enum MessageType
{
    info,
    success,
    warning,
    error
}

public enum ORDSTATUS
{
    PLACED,
    CANCELLED,
    CONFIRMED,
    HOLD,
    INPROCESS    
}