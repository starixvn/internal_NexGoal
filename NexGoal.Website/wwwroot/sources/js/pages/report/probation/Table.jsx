import React, { Component } from 'react';
import moment from 'moment';

import getResource from '../../../resourceHelper';
import { noDataIndication, resolveClientUrl } from '../../../common-functions';
import { PerformanceProcess, DateFormat } from '../../../constants';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.today = moment();
    }

    handleShowComment = event => {
        this.props.onShowComment(event.target.dataset.comment);
    }

    renderRow = (probationer, index) => {
        const evaluatorResult = probationer.isQualifiedByEvaluator !== null ? probationer.isQualifiedByEvaluator ? 'Passed' : 'Failed' : '-';
        const finalResult = probationer.isQualified !== null ? probationer.isQualified ? 'Passed' : 'Failed' : '-';
        return (
            <tr key={probationer.employeeId}>
                <td className="text-center">{index + 1}</td>
                <td>{probationer.nickName}</td>
                <td>{probationer.fullName}</td>
                <td>{probationer.joinedDate.format(DateFormat)}</td>
                <td>{probationer.groupName}</td>
                <td>{probationer.jobTitle}</td>
                <td className="text-center">{probationer.selfScore || '-'}</td>
                <td className="text-center">
                    {probationer.selfComment ? <span className="icon-search" data-comment={probationer.selfComment} onClick={this.handleShowComment} /> : '-'}
                </td>
                <td className="text-center">{probationer.evaluatorScore || '-'}</td>
                <td className="text-center"><span className={evaluatorResult.toLowerCase()} >{evaluatorResult}</span></td>
                <td className="text-center">
                    {probationer.evaluatorComment ? <span className="icon-search" data-comment={probationer.evaluatorComment} onClick={this.handleShowComment} /> : '-'}
                </td>
                <td className="text-center">
                    <span className={finalResult.toLowerCase()}>{finalResult}</span>
                </td>
                <td className="text-center">
                    {probationer.reviewerComment ? <span className="icon-search" data-comment={probationer.reviewerComment} onClick={this.handleShowComment} /> : '-'}
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
                            <th colSpan="3">{getResource('Evaluator')}</th>
                            <th colSpan="2">{getResource('Reviewer')}</th>
                            <th rowSpan="2">{getResource('Action')}</th>

                        </tr>
                        <tr>
                            <th>{getResource('Score')}</th>
                            <th>{getResource('Comment')}</th>
                            <th>{getResource('Score')}</th>
                            <th>{getResource('Result')}</th>
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