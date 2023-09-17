// -----------------------------------------------------------------------
// <copyright file="DiaryRepositoryTest.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Entities;
using Diary_Sample.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Diary_Sample_Test.Repositories;

public class DiaryRepositoryTest
{
    private readonly Mock<ILogger<DiaryRepository>> mockLogger;
    private readonly Mock<IConfiguration> mockConfiguration;
    private readonly Mock<DiarySampleContext> mockContext;
    private readonly Mock<DbSet<Diary>> mockSet;
    private readonly Exception mockException;
    private readonly DiaryRepository repository;
    public DiaryRepositoryTest()
    {
        mockLogger = new Mock<ILogger<DiaryRepository>>() { CallBase = true };
        mockConfiguration = new Mock<IConfiguration>();
        mockContext = new Mock<DiarySampleContext>(mockConfiguration.Object);
        mockSet = new Mock<DbSet<Diary>>();
        // MySqlExceptionは直接moqで作れないのでリフレクションで作る
        TypeInfo mySqlExceptionType = typeof(MySql.Data.MySqlClient.MySqlException).GetTypeInfo();
        ConstructorInfo internalConstructor = (from consInfo in mySqlExceptionType.DeclaredConstructors
                                               let paramInfos = consInfo.GetParameters()
                                               where paramInfos.Length == 1 && paramInfos[0].ParameterType == typeof(string)
                                               select consInfo).Single();
        mockException = internalConstructor.Invoke(new[] { "DBエラー" }) as Exception;

        repository = new DiaryRepository(mockLogger.Object, mockContext.Object);
    }

    [Fact]
    public void CreateTest1()
    {
        // 正常系
        Diary diary = new Diary("タイトル", "本文");
        mockContext.Setup(x => x.Add(diary));
        mockContext.Setup(x => x.SaveChanges()).Returns(1);

        // Createメソッドのテスト
        bool result = repository.create(diary);

        // 返却値の確認
        Assert.True(result);
    }
    [Fact]
    public void CreateTest2()
    {
        // ありえないケース（未登録）
        Diary diary = new Diary("タイトル", "本文");
        mockContext.Setup(x => x.Add(diary));
        mockContext.Setup(x => x.SaveChanges()).Returns(0);

        // Createメソッドのテスト
        bool result = repository.create(diary);

        // 返却値の確認
        Assert.False(result);
    }
    [Fact]
    public void CreateTest3()
    {
        // 異常系（DBエラー）
        Diary diary = new Diary("タイトル", "本文");
        mockContext.Setup(x => x.Add(diary));
        mockContext.Setup(x => x.SaveChanges()).Throws(mockException);

        // Createメソッドのテスト
        var exception = Assert.Throws<MySqlException>(() =>
        {
            repository.create(diary);
        });

        // ログが2回出力されていること
        mockLogger.Verify(
            x =>
            x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Exactly(2));

        // 例外メッセージの確認
        Assert.Equal("DBエラー", exception.Message);

    }
    [Fact]
    public void ReadTest1()
    {
        // 正常系
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary("AAA", "テスト"),
            new Diary("BBB", "テスト"),
            new Diary("CCC", "テスト"),
            new Diary("DDD", "テスト"),
            new Diary("EEE", "テスト"),
            new Diary("FFF", "テスト"),
        }.AsQueryable();
        SetIQueryable(data);

        mockContext.Setup(x => x.diary).Returns(mockSet.Object);

        // readメソッドのテスト（1ページ目）
        List<Diary> result = repository.read(1, 5);

        // 返却値の確認
        Assert.Equal(5, result.Count);
        // 登録日時の降順に並ぶこと、要素は5つまで取得されること
        Assert.Equal("FFF", result[0].title);
        Assert.Equal("EEE", result[1].title);
        Assert.Equal("DDD", result[2].title);
        Assert.Equal("CCC", result[3].title);
        Assert.Equal("BBB", result[4].title);

        // readメソッドのテスト（2ページ目）
        result = repository.read(2, 5);

        // 返却値の確認
        Assert.Single(result);
        // 残りの要素が取得されること
        Assert.Equal("AAA", result[0].title);
    }

    [Fact]
    public void ReadTest2()
    {
        // 正常系
        IQueryable<Diary> data = new List<Diary> { }.AsQueryable();
        SetIQueryable(data);

        mockContext.Setup(x => x.diary).Returns(mockSet.Object);

        // readメソッドのテスト（0件）
        List<Diary> result = repository.read(1, 5);

        // 返却値の確認
        Assert.Empty(result);
    }
    [Fact]
    public void ReadTest3()
    {
        // 異常系
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary("AAA", "テスト"),
            new Diary("BBB", "テスト"),
            new Diary("CCC", "テスト"),
            new Diary("DDD", "テスト"),
            new Diary("EEE", "テスト"),
            new Diary("FFF", "テスト"),
        }.AsQueryable();
        SetIQueryable(data);

        mockContext.Setup(x => x.diary).Throws(mockException);

        // readメソッドのテスト
        var exception = Assert.Throws<MySqlException>(() =>
        {
            repository.read(1, 5);
        });
        // ログが2回出力されていること
        mockLogger.Verify(
            x =>
            x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Exactly(2));

        // 例外メッセージの確認
        Assert.Equal("DBエラー", exception.Message);

    }
    [Fact]
    public void ReadCountTest1()
    {
        // 正常系
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary("AAA", "テスト"),
            new Diary("BBB", "テスト"),
            new Diary("CCC", "テスト"),
            new Diary("DDD", "テスト"),
            new Diary("EEE", "テスト"),
        }.AsQueryable();
        SetIQueryable(data);

        mockContext.Setup(x => x.diary).Returns(mockSet.Object);

        // readCountメソッドのテスト
        int result = repository.readCount();

        // 返却値の確認
        Assert.Equal(5, result);
    }
    [Fact]
    public void ReadCountTest2()
    {
        // 正常系
        IQueryable<Diary> data = new List<Diary> { }.AsQueryable();
        SetIQueryable(data);

        mockContext.Setup(x => x.diary).Returns(mockSet.Object);

        // readCountメソッドのテスト（0件）
        int result = repository.readCount();

        // 返却値の確認
        Assert.Equal(0, result);
    }
    [Fact]
    public void ReadCountTest3()
    {
        // 異常系
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary("AAA", "テスト"),
            new Diary("BBB", "テスト"),
            new Diary("CCC", "テスト"),
            new Diary("DDD", "テスト"),
            new Diary("EEE", "テスト"),
            new Diary("FFF", "テスト"),
        }.AsQueryable();
        SetIQueryable(data);

        mockContext.Setup(x => x.diary).Throws(mockException);

        // readCountメソッドのテスト
        var exception = Assert.Throws<MySqlException>(() =>
        {
            repository.readCount();
        });

        // ログが2回出力されていること
        mockLogger.Verify(
            x =>
            x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Exactly(2));

        // 例外メッセージの確認
        Assert.Equal("DBエラー", exception.Message);
    }

    // 正常系：データありのキー値を指定
    [Fact]
    public void ReadTest001()
    {
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary(1, "たいとる１（てすと）", "ほんぶん１（てすと）"),
            new Diary(2, "たいとる２（てすと）", "ほんぶん２（てすと）"),
            new Diary(3, "たいとる３（てすと）", "ほんぶん３（てすと）"),
            new Diary(4, "たいとる４（てすと）", "ほんぶん４（てすと）"),
            new Diary(5, "たいとる５（てすと）", "ほんぶん５（てすと）"),
            new Diary(6, "たいとる６（てすと）", "ほんぶん６（てすと）"),
            new Diary(7, "たいとる７（てすと）", "ほんぶん７（てすと）"),
        }.AsQueryable();
        SetIQueryable(data);
        mockContext.Setup(x => x.diary).Returns(mockSet.Object);

        var resultList = repository.Read(3);

        Assert.Single(resultList);
        var result = resultList[0];
        Assert.Equal(3, result.id);
        Assert.Equal("たいとる３（てすと）", result.title);
        Assert.Equal("ほんぶん３（てすと）", result.content);
    }

    // 正常系：データなしのキー値を指定
    [Fact]
    public void ReadTest002()
    {
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary(1, "たいとる１（てすと）", "ほんぶん１（てすと）"),
            new Diary(2, "たいとる２（てすと）", "ほんぶん２（てすと）"),
            new Diary(3, "たいとる３（てすと）", "ほんぶん３（てすと）"),
            new Diary(4, "たいとる４（てすと）", "ほんぶん４（てすと）"),
            new Diary(5, "たいとる５（てすと）", "ほんぶん５（てすと）"),
            new Diary(6, "たいとる６（てすと）", "ほんぶん６（てすと）"),
            new Diary(7, "たいとる７（てすと）", "ほんぶん７（てすと）"),
        }.AsQueryable();
        SetIQueryable(data);
        mockContext.Setup(x => x.diary).Returns(mockSet.Object);

        var resultList = repository.Read(8);

        Assert.Empty(resultList);
    }

    // 異常系：MySqlException
    [Fact]
    public void ReadTest003()
    {
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary(1, "たいとる１（てすと）", "ほんぶん１（てすと）"),
            new Diary(2, "たいとる２（てすと）", "ほんぶん２（てすと）"),
            new Diary(3, "たいとる３（てすと）", "ほんぶん３（てすと）"),
            new Diary(4, "たいとる４（てすと）", "ほんぶん４（てすと）"),
            new Diary(5, "たいとる５（てすと）", "ほんぶん５（てすと）"),
            new Diary(6, "たいとる６（てすと）", "ほんぶん６（てすと）"),
            new Diary(7, "たいとる７（てすと）", "ほんぶん７（てすと）"),
        }.AsQueryable();
        SetIQueryable(data);
        mockContext.Setup(x => x.diary).Throws(mockException);

        var exception = Assert.Throws<MySqlException>(() =>
        {
            var resultList = repository.Read(1);
        });
        Assert.Equal("DBエラー", exception.Message);
        mockLogger.Verify(
            x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Exactly(2));
    }

    // 正常系：レコード更新
    [Fact]
    public void UpdateTest001()
    {
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary(1, "たいとる１（てすと）", "ほんぶん１（てすと）"),
            new Diary(2, "たいとる２（てすと）", "ほんぶん２（てすと）"),
            new Diary(3, "たいとる３（てすと）", "ほんぶん３（てすと）"),
            new Diary(4, "たいとる４（てすと）", "ほんぶん４（てすと）"),
            new Diary(5, "たいとる５（てすと）", "ほんぶん５（てすと）"),
            new Diary(6, "たいとる６（てすと）", "ほんぶん６（てすと）"),
            new Diary(7, "たいとる７（てすと）", "ほんぶん７（てすと）"),
        }.AsQueryable();
        SetIQueryable(data);
        mockContext.Setup(x => x.diary).Returns(mockSet.Object);
        Diary diary = new Diary(2, "タイトル２（てすと）を更新", "ほんぶん２（てすと）を更新");
        mockContext.Setup(x => x.SaveChanges()).Returns(1);

        bool result = repository.Update(diary);

        Assert.True(result);
    }

    // 準正常系：存在しないレコードを更新
    [Fact]
    public void UpdateTest002()
    {
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary(1, "たいとる１（てすと）", "ほんぶん１（てすと）"),
            new Diary(2, "たいとる２（てすと）", "ほんぶん２（てすと）"),
            new Diary(3, "たいとる３（てすと）", "ほんぶん３（てすと）"),
            new Diary(4, "たいとる４（てすと）", "ほんぶん４（てすと）"),
            new Diary(5, "たいとる５（てすと）", "ほんぶん５（てすと）"),
            new Diary(6, "たいとる６（てすと）", "ほんぶん６（てすと）"),
            new Diary(7, "たいとる７（てすと）", "ほんぶん７（てすと）"),
        }.AsQueryable();
        SetIQueryable(data);
        mockContext.Setup(x => x.diary).Returns(mockSet.Object);
        Diary diary = new Diary(8, "タイトル８（てすと）を更新", "ほんぶん８（てすと）を更新");
        mockContext.Setup(x => x.SaveChanges()).Returns(1);

        bool result = repository.Update(diary);

        Assert.False(result);
        mockLogger.Verify(
                    x => x.Log(
              LogLevel.Error,
              It.IsAny<EventId>(),
              It.IsAny<It.IsAnyType>(),
              It.IsAny<Exception>(),
              (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Exactly(2));
    }

    // 異常系：MySqlException
    [Fact]
    public void UpdateTest003()
    {
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary(1, "たいとる１（てすと）", "ほんぶん１（てすと）"),
            new Diary(2, "たいとる２（てすと）", "ほんぶん２（てすと）"),
            new Diary(3, "たいとる３（てすと）", "ほんぶん３（てすと）"),
            new Diary(4, "たいとる４（てすと）", "ほんぶん４（てすと）"),
            new Diary(5, "たいとる５（てすと）", "ほんぶん５（てすと）"),
            new Diary(6, "たいとる６（てすと）", "ほんぶん６（てすと）"),
            new Diary(7, "たいとる７（てすと）", "ほんぶん７（てすと）"),
        }.AsQueryable();
        SetIQueryable(data);
        mockContext.Setup(x => x.diary).Returns(mockSet.Object);
        Diary diary = new Diary(1, "タイトル１（てすと）を更新", "ほんぶん１（てすと）を更新");
        mockContext.Setup(x => x.SaveChanges()).Throws(mockException);

        var exception = Assert.Throws<MySqlException>(() =>
        {
            bool result = repository.Update(diary);
        });
        Assert.Equal("DBエラー", exception.Message);

        mockLogger.Verify(
            x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Exactly(2));
    }

    // 正常系：レコード削除
    [Fact]
    public void DeleteTest001()
    {
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary(1, "たいとる１（てすと）", "ほんぶん１（てすと）"),
            new Diary(2, "たいとる２（てすと）", "ほんぶん２（てすと）"),
            new Diary(3, "たいとる３（てすと）", "ほんぶん３（てすと）"),
            new Diary(4, "たいとる４（てすと）", "ほんぶん４（てすと）"),
            new Diary(5, "たいとる５（てすと）", "ほんぶん５（てすと）"),
            new Diary(6, "たいとる６（てすと）", "ほんぶん６（てすと）"),
            new Diary(7, "たいとる７（てすと）", "ほんぶん７（てすと）"),
        }.AsQueryable();
        SetIQueryable(data);
        mockContext.Setup(x => x.diary).Returns(mockSet.Object);
        mockContext.Setup(x => x.SaveChanges()).Returns(1);

        bool result = repository.Delete(2);

        Assert.True(result);
    }

    // 準正常系：存在しないレコードを削除
    [Fact]
    public void DeleteTest002()
    {
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary(1, "たいとる１（てすと）", "ほんぶん１（てすと）"),
            new Diary(2, "たいとる２（てすと）", "ほんぶん２（てすと）"),
            new Diary(3, "たいとる３（てすと）", "ほんぶん３（てすと）"),
            new Diary(4, "たいとる４（てすと）", "ほんぶん４（てすと）"),
            new Diary(5, "たいとる５（てすと）", "ほんぶん５（てすと）"),
            new Diary(6, "たいとる６（てすと）", "ほんぶん６（てすと）"),
            new Diary(7, "たいとる７（てすと）", "ほんぶん７（てすと）"),
        }.AsQueryable();
        SetIQueryable(data);
        mockContext.Setup(x => x.diary).Returns(mockSet.Object);
        mockContext.Setup(x => x.SaveChanges()).Returns(1);

        bool result = repository.Delete(8);

        Assert.False(result);
        mockLogger.Verify(
                    x => x.Log(
              LogLevel.Error,
              It.IsAny<EventId>(),
              It.IsAny<It.IsAnyType>(),
              It.IsAny<Exception>(),
              (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                    Times.Exactly(2));
    }

    // 異常系：MySqlException
    [Fact]
    public void DeleteTest003()
    {
        IQueryable<Diary> data = new List<Diary>
        {
            new Diary(1, "たいとる１（てすと）", "ほんぶん１（てすと）"),
            new Diary(2, "たいとる２（てすと）", "ほんぶん２（てすと）"),
            new Diary(3, "たいとる３（てすと）", "ほんぶん３（てすと）"),
            new Diary(4, "たいとる４（てすと）", "ほんぶん４（てすと）"),
            new Diary(5, "たいとる５（てすと）", "ほんぶん５（てすと）"),
            new Diary(6, "たいとる６（てすと）", "ほんぶん６（てすと）"),
            new Diary(7, "たいとる７（てすと）", "ほんぶん７（てすと）"),
        }.AsQueryable();
        SetIQueryable(data);
        mockContext.Setup(x => x.diary).Returns(mockSet.Object);
        mockContext.Setup(x => x.SaveChanges()).Throws(mockException);

        var exception = Assert.Throws<MySqlException>(() =>
        {
            bool result = repository.Delete(1);
        });
        Assert.Equal("DBエラー", exception.Message);

        mockLogger.Verify(
            x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Exactly(2));
    }

    private void SetIQueryable(IQueryable<Diary> data)
    {
        mockSet.As<IQueryable<Diary>>().Setup(x => x.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<Diary>>().Setup(x => x.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<Diary>>().Setup(x => x.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<Diary>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
    }
}