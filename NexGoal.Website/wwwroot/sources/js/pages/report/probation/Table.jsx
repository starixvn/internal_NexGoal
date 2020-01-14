import React, { Component } from 'react';
import moment from 'moment';

import getResource from '../../../resourceHelper';
import { noDataIndication, resolveClientUrl } from '../../../common-functions';
import { PerformanceProcess, DateFormat, EmptyPlaceHolder } from '../../../constants';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.today = moment();
    }

    handleShowComment = event => {
        this.props.onShowComment(event.target.dataset.comment, parseInt(event.target.dataset.processid, 10));
    }

    renderPerformanceResult = isQualified => {
        return (
            <span className={isQualified ? 'passed' : 'failed'}>
                {isQualified !== null ? isQualified ? 'Passed' : 'Failed' : EmptyPlaceHolder}
            </span>
        );
    }

    renderRow = (probationer, index) => {
        let evaluatorResult = EmptyPlaceHolder;
        if (probationer.canViewEvaluatorResult) {
            evaluatorResult = this.renderPerformanceResult(probationer.isQualifiedByEvaluator);
        }

        return (
            <tr key={probationer.employeeId}>
                <td className="text-center">{index + 1}</td>
                <td>{probationer.nickName}</td>
                <td>{probationer.fullName}</td>
                <td>{probationer.joinedDate.format(DateFormat)}</td>
                <td>{probationer.groupName}</td>
                <td>{probationer.jobTitle}</td>
                <td className="text-center">{probationer.selfScore || EmptyPlaceHolder}</td>
                <td className="text-center">
                    {
                        probationer.selfComment ?
                            <span className="icon-search" data-processid={PerformanceProcess.Self}
                                data-comment={probationer.selfComment} onClick={this.handleShowComment}
                            />
                            : EmptyPlaceHolder
                    }
                </td>
                <td className="text-center">{probationer.evaluatorScore || EmptyPlaceHolder}</td>
                {
                    this.props.showEvaluatorResult &&
                    <td className="text-center">{evaluatorResult}</td>
                }
                <td className="text-center">
                    {
                        probationer.evaluatorComment ?
                            <span className="icon-search" data-processid={PerformanceProcess.Evaluator}
                                data-comment={probationer.evaluatorComment} onClick={this.handleShowComment}
                            />
                            : EmptyPlaceHolder
                    }
                </td>
                <td className="text-center">{this.renderPerformanceResult(probationer.isQualified)}</td>
                <td className="text-center">
                    {
                        probationer.reviewerComment ?
                            <span className="icon-search" data-processid={PerformanceProcess.Reviewer}
                                data-comment={probationer.reviewerComment} onClick={this.handleShowComment}
                            />
                            : EmptyPlaceHolder
                    }
                </td>
                <td className="text-center">
                    <a href={resolveClientUrl(`ProbationReport/Detail/${probationer.employeeId}`)} >
                        <span className="icon-search" />
                    </a>
                    <a href={resolveClientUrl(`ProbationReport/ExportPdf/${probationer.employeeId}`)} >
                        <span className="icon-pdf" />
                    </a>
                </td>
            </tr>
        );
    }

    render() {
        if (this.props.tableData) {
            return (
                <table className="table table-bordered" >
                    <thead>
                        <tr>
                            <th rowSpan="2">{getResource('SymbolCount')}</th>
                            <th rowSpan="2">{getResource('NickName')}</th>
                            <th rowSpan="2">{getResource('FullName')}</th>
                            <th rowSpan="2">{getResource('JoinedDate')}</th>
                            <th rowSpan="2">{getResource('Team')}</th>
                            <th rowSpan="2">{getResource('JobTitle')}</th>
                            <th colSpan="2">{getResource('Self')}</th>
                            <th colSpan={this.props.showEvaluatorResult ? 3 : 2}>{getResource('Evaluator')}</th>
                            <th colSpan="2">{getResource('Reviewer')}</th>
                            <th rowSpan="2">{getResource('Action')}</th>

                        </tr>
                        <tr>
                            <th>{getResource('Score')}</th>
                            <th>{getResource('Comment')}</th>
                            <th>{getResource('Score')}</th>
                            {this.props.showEvaluatorResult &&
                                <th>{getResource('Result')}</th>
                            }
                            <th>{getResource('Comment')}</th>
                            <th>{getResource('FinalResult')}</th>
                            <th>{getResource('Comment')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.props.tableData.length > 0 ? this.props.tableData.map(this.renderRow)
                            : <tr><td className="text-center" colSpan={15}>{noDataIndication()}</td></tr>}
                    </tbody>
                </table>
            );
        }

        return '';
    }
}