-- 自動建立會員編號

create trigger M_AutoNumber
on Members
after insert
as
begin
    declare @one char(1), 
            @exid char(5),
            @id varchar(4)     
        select top 1 @exid=MemberID from Members order by MemberID desc
        set @one=left(@exid,1)
        set @id=cast(right(@exid,4) as char)+1
        update Members set MemberID=@one+right('000'+cast(@id as varchar),4) where MemberID='M0000'
end
go


--自動建立公告編號

create trigger B_AutoNumber
on Bulletins
after insert
as
begin
    declare @one char(1), 
            @exid char(5),
            @id varchar(4)     
        select top 1 @exid=BulletinID from Bulletins order by BulletinID desc
        set @one=left(@exid,1)
        set @id=cast(right(@exid,4) as char)+1
        update Bulletins set BulletinID=@one+right('000'+cast(@id as varchar),4) where BulletinID='N0000'
end
go

--驗證Email是否重複

create TRIGGER IsEmailUni
ON Members
for INSERT
AS
begin
 declare @email nvarchar(320)
 declare @key varchar(64)
 select @email=Email,@key=MemberID from inserted

 IF ((select count(*) from Members where Email = @email)>1)
 begin 
  delete from Members where MemberID=@key
  raiserror ('該Email使用者已存在!',16,1)
  return
 end
end

