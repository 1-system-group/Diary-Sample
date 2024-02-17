// -----------------------------------------------------------------------
// <copyright file="IApiController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Threading.Tasks;
using Diary_Sample.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diary_Sample.Controllers
{
    public interface IApiController
    {
        /// <summary>
        /// ログインする
        /// </summary>
        /// <remarks>
        /// モバイルアプリからログインするためのAPI
        /// </remarks>
        /// <param name="model"></param>
        /// <returns>認証トークン</returns>
        /// <response code="200">OK 認証トークン</response>
        /// <response code="401">NG 認証失敗</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        public ActionResult<string> Login([FromBody] AuthModel model);

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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<int> Counts();

        /// <summary>
        /// 日記を取得する
        /// </summary>
        /// <remarks>
        /// 登録されている日記のうち、引数のid(連番)に紐づく日記の情報を取得する。
        /// id(連番)に紐づく日記が見つからない場合、404(NotFound)を返す。
        /// </remarks>
        /// <param name="id">連番</param>
        /// <returns>日記</returns>
        /// <response code="200">OK 日記</response>
        /// <response code="404">NG 日記</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DiaryModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        public ActionResult<DiaryModel> Diary(int id);
    }
}