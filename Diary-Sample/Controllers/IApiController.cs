// -----------------------------------------------------------------------
// <copyright file="IApiController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using Diary_Sample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diary_Sample.Controllers
{
    public interface IApiController
    {
        /// <summary>
        /// 日記の情報一覧を取得する
        /// </summary>
        /// <remarks>
        /// 1ページの件数は10件。ページ番号は1以上の値を指定。存在しないページ番号を指定すると空リストが返却される。
        /// </remarks>
        /// <param name="page">ページ番号</param>
        /// <returns>日記の情報リスト</returns>
        /// <response code="200">OK 日記の情報リスト</response>
        /// <response code="400">NG 不正なページ指定</response>
        [HttpGet("{page}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        public ActionResult<List<DiaryRow>> Lists(int page);

        /// <summary>
        /// 日記の件数を取得する
        /// </summary>
        /// <remarks>
        /// 登録されている日記の件数を取得する。本件数から何ページ存在するか判断する。
        /// </remarks>
        /// <returns>日記の件数</returns>
        /// <response code="200">OK 日記の件数</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<int> Counts();
    }
}