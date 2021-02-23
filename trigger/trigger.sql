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

go
--自動建立群組編號

create trigger G_AutoNumber
on Groups
after insert
as
begin
    declare @one char(1), 
            @exid char(5),
            @id varchar(4)     
        select top 1 @exid=GroupID from Groups order by GroupID desc
        set @one=left(@exid,1)
        set @id=cast(right(@exid,4) as char)+1
        update Groups set GroupID =@one+right('000'+cast(@id as varchar),4) where GroupID='G0000'
end
go

-- 自動建立project ID
create trigger AutoSetProjectID
on Projects
after insert
as
begin
    declare 
        @one char(1), 
        @exid char(5),
        @id varchar(4)     
    select top 1 @exid = ProjectID 
    from Projects 
    order by ProjectID desc
    set @one = left(@exid, 1)
    set @id  = cast(right(@exid, 4) as char) + 1
    update Projects 
    set ProjectID = @one + right('000' + cast(@id as varchar), 4) 
    where ProjectID = 'P0000'
end
go

-- 自動建立 Board ID
create trigger AutoSetBoardID
on Boards
after insert
as
begin
    declare 
        @one char(1), 
        @exid char(5),
        @id varchar(4)     
    select top 1 @exid = BoardID 
    from Boards 
    order by BoardID desc
    set @one = left(@exid, 1)
    set @id  = cast(right(@exid, 4) as char) + 1
    update Boards 
    set BoardID = @one + right('000' + cast(@id as varchar), 4) 
    where BoardID = 'B0000'
end
go

CREATE trigger VoteRecords_checkVoting
on VoteRecords
AFTER UPDATE
as
BEGIN
    declare 
        @voteId int,
        @result nvarchar(MAX)

    SELECT @voteId = VoteID FROM inserted
    
    IF ((SELECT Result FROM Votes WHERE VoteID = @voteId) is null)
    BEGIN
        IF ((SELECT VoteCount FROM Votes WHERE VoteID = @voteId) 
            = (SELECT SUM(VoteCounts) FROM VoteRecords WHERE VoteID = @voteId))
        BEGIN
            SELECT TOP 1 @result = Choice
            FROM VoteRecords
            WHERE VoteID = @voteId AND VoteCounts = (
                SELECT MAX(VoteCounts)
                FROM VoteRecords
                WHERE VoteID = @voteId
            )
            ORDER BY NEWID()
            UPDATE Votes SET Result = @result WHERE VoteID = @voteId
        END
    END
END
go
---- 檢查投票是否已結束
CREATE trigger Votes_checkVoting
on Votes
AFTER UPDATE
as
BEGIN
    declare 
        @voteId int,
        @result nvarchar(MAX)

    SELECT @voteId = VoteID FROM inserted
    
    IF ((SELECT Result FROM Votes WHERE VoteID = @voteId) is null)
    BEGIN
        IF ((SELECT VoteCount FROM Votes WHERE VoteID = @voteId) 
            = (SELECT SUM(VoteCounts) FROM VoteRecords WHERE VoteID = @voteId))
        BEGIN
            SELECT TOP 1 @result = Choice
            FROM VoteRecords
            WHERE VoteID = @voteId AND VoteCounts = (
                SELECT MAX(VoteCounts)
                FROM VoteRecords
                WHERE VoteID = @voteId
            )
            ORDER BY NEWID()
            UPDATE Votes SET Result = @result WHERE VoteID = @voteId
        END
    END
END
go
