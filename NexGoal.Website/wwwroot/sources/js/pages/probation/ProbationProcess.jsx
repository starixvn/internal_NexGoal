import React, { PureComponent } from 'react';
import moment from 'moment';

import getResource from '../../resourceHelper';
import { DateFormat } from '../../constants';

export default class ProbationProcess extends PureComponent {
    today = moment();

    renderEvaluator = process => {
        return (
            <td key={process.processId} className="text-center ">
                <strong>{`${process.process}: ${process.evaluatorNickName}`}</strong>
            </td>
        );
    }

    renderProcess = process => {
        const status = this.today.isSameOrAfter(process.fromDate, 'day') && this.today.isSameOrBefore(process.toDate, 'day') ?
            'in-process' : this.today.isAfter(process.toDate, 'day') ? 'end-process' : 'open';

        return (
            <td key={process.processId} className="text-center">
                <div><strong>{`${process.fromDate.format(DateFormat)} - ${process.toDate.format(DateFormat)}`}</strong></div>
                {
                    status === 'end-process' &&
                    <div>
                        <span className="label-status label-completed">{getResource('Completed')}</span>
                    </div>
                }
                {
                    status === 'in-process' &&
                    <div>
                        <span className="label-status label-inprogress">{getResource('InProgress')}</span>
                    </div>
                }
                {
                    status === 'open' &&
                    <div>
                        <span className="label-status label-notready">{getResource('Open')}</span>
                    </div>
                }
            </td>
        );
    }

    render() {
        return (
            <table className="table table-bordered process">
                <tbody>
                    <tr>
                        <td colSpan={3} className="title">{getResource('EvaluationProcess')}</td>
                    </tr>
                    <tr className="group-row">
                        {this.props.processes.map(this.renderEvaluator)}
                    </tr>
                    <tr>
                        {this.props.processes.map(this.renderProcess)}
                    </tr>
                </tbody>
            </table>
        );
    }
}