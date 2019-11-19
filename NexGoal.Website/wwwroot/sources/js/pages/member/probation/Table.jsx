import React, { Component } from 'react';
import moment from 'moment';

import getResource from '../../../resourceHelper';
import { noDataIndication } from '../../../common-functions';
import { PerformanceProcess } from '../../../constants';

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.today = moment();
    }

    handleShowComment = event => {
        this.props.onShowComment(event.target.dataset.comment, parseInt(event.target.dataset.processid, 10));
    }

    handleRevisedChange = event => {
        const probationer = this.props.tableData.find(x => x.employeeId === parseInt(event.target.dataset.employeeid, 10));

        const reviewerProcess = probationer.settings.find(x => x.processId === PerformanceProcess.Reviewer);

        if (probationer.isQualified !== null || !probationer.processIds.some(p => p === PerformanceProcess.Reviewer)
            || this.today.isBefore(reviewerProcess.fromDate) || this.today.isAfter(reviewerProcess.toDate)) {
            event.preventDefault();
        } else {
            this.props.onConfirmedByReviewer(probationer);
        }
    }

    renderRow = probationer => {
        let evaluatorResult = '-', finalResult = '-';

        if (probationer.isQualifiedByEvaluator !== null) {
            evaluatorResult = probationer.isQualifiedByEvaluator ? 'Passed' : 'Failed';
        }

        if (probationer.isQualified !== null) {
            finalResult = probationer.isQualified ? 'Passed' : 'Failed';
        }

        return (
            <tr key={probationer.employeeId}>
                <td>{probationer.nickName}</td>
                <td className="text-center">{probationer.selfScore || '-'}</td>
                <td className="text-center">
                    {probationer.selfComment ?
                        <span className="icon-search" data-processid={PerformanceProcess.Self}
                            data-comment={probationer.selfComment} onClick={this.handleShowComment}
                        />
                        : '-'
                    }
                </td>
                <td className="text-center">{probationer.evaluatorScore || '-'}</td>
                <td className="text-center">
                    <span className={evaluatorResult.toLowerCase()} >{evaluatorResult}</span>
                </td>
                <td className="text-center">
                    {probationer.evaluatorComment ?
                        <span className="icon-search" data-processid={PerformanceProcess.Evaluator}
                            data-comment={probationer.evaluatorComment} onClick={this.handleShowComment}
                        />
                        : '-'
                    }
                </td>
                <td className="text-center">
                    <span className={finalResult.toLowerCase()}>{finalResult}</span>
                </td>
                <td className="text-center">
                    {probationer.reviewerComment ?
                        <span className="icon-search" data-processid={PerformanceProcess.Reviewer}
                            data-comment={probationer.reviewerComment} onClick={this.handleShowComment}
                        />
                        : '-'
                    }
                </td>
                <td className="text-center">
                    <input type="checkbox" onChange={this.handleRevisedChange} data-employeeid={probationer.employeeId}
                        checked={probationer.isQualified !== null} id={`chb-${probationer.employeeId}`} className="checkbox-custom"
                    />
                    <label htmlFor={`chb-${probationer.employeeId}`} />
                </td>
                <td className="text-center">
                    <a className="fa fa-pencil" title="Edit" href={`/Probation/Evaluate/${probationer.employeeId}`} />
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
                            <th rowSpan="2">{getResource('NickName')}</th>
                            <th colSpan="2">{getResource('Self')}</th>
                            <th colSpan="3">{getResource('Evaluator')}</th>
                            <th colSpan="2">{getResource('Reviewer')}</th>
                            <th rowSpan="2">{getResource('Revised')}</th>
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
                            : <tr><td className="text-center" colSpan={10}>{noDataIndication()}</td></tr>}
                    </tbody>
                </table>
            );
        }

        return '';
    }
}