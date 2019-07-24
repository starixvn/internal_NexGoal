import React, { PureComponent } from 'react';
import moment from 'moment';

import getResource from '../../resourceHelper';
import { DateFormat } from '../../constants';

export default class ProbationProcess extends PureComponent {
    today = moment();

    renderEvaluator = process => {
        return (
            <td key={process.processId} className="text-center">
                {`${process.process}: ${process.evaluatorNickName}`}
            </td>
        );
    }

    renderProcess = process => {
        const status = this.today.isSameOrAfter(process.fromDate, 'day') && this.today.isSameOrBefore(process.toDate, 'day') ?
            'in-process' : this.today.isAfter(process.toDate, 'day') ? 'end-process' : 'open';

        return (
            <td key={process.processId} className="text-center">
                <div>{`${process.fromDate.format(DateFormat)} - ${process.toDate.format(DateFormat)}`}</div>
                {
                    status === 'end-process' &&
                    <div>
                        <span>{getResource('Completed')}</span>
                    </div>
                }
                {
                    status === 'in-process' &&
                    <div>
                        <span>{getResource('InProgress')}</span>
                    </div>
                }
                {
                    status === 'open' &&
                    <div>
                        <span>{getResource('NotReady')}</span>
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
                    <tr>
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