using System;
using System.Collections.Generic;
using System.Text;
using Diary_Sample.Entities;
using Diary_Sample.Models;
using Diary_Sample.Repositories;
using Diary_Sample.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Diary_Sample_Test.Services
{
    public class ReferServiceTest
    {
        private readonly Mock<IDiaryRepository> mockRepository;

        public ReferServiceTest()
        {
            mockRepository = new Mock<IDiaryRepository>();
        }

        // 正常系：レコード取得成功
        [Fact]
        public void GetDiaryTest001()
        {
            var diaryList = new List<Diary>();
            diaryList.Add(new Diary(1, "たいとる（てすと）", "ほんぶん（てすと）"));
            mockRepository.Setup(x => x.Read(1)).Returns(diaryList);
            var service = new ReferService(mockRepository.Object);

            var result = service.GetDiary(1);
            var model = Assert.IsType<ReferViewModel>(result);
            Assert.Equal("1", result.Id);
            Assert.Equal("たいとる（てすと）", result.Title);
            Assert.Equal("ほんぶん（てすと）", result.Content);
        }

        // 異常系：レコード取得失敗
        [Fact]
        public void GetDiaryTest002()
        {
            var diaryList = new List<Diary>();
            mockRepository.Setup(x => x.Read(1)).Returns(diaryList);
            var service = new ReferService(mockRepository.Object);

            var result = service.GetDiary(1);

            Assert.Null(result);
        }
    }
}