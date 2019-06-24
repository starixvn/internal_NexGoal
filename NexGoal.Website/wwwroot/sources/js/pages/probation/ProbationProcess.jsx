import React, { PureComponent } from 'react';
import moment from 'moment';

import getResource from '../../resourceHelper';
import { DateFormat } from '../../constants';

export default class ProbationProcess extends PureComponent {
    today = moment();

    renderProcess = process => {
        const status = this.today.isSameOrAfter(process.fromDate, 'day') && this.today.isSameOrBefore(process.toDate, 'day') ?
            'in-process' : this.today.isAfter(process.toDate, 'day') ? 'end-process' : 'open';

        return (
            <li key={process.processId} className={`process-item ${status}`}>
                <div>{`${process.process}: ${process.evaluatorNickName}`}</div>
                <div>{`${process.fromDate.format(DateFormat)} - ${process.toDate.format(DateFormat)}`}</div>
                {
                    status === 'end-process' &&
                    <div>
                        <span>{getResource('Completed')}</span> <i className="fa fa-check-square-o" />
                    </div>
                }
                {
                    status === 'in-process' &&
                    <div>
                        <span>{getResource('InProgress')}</span> <i className="fa fa-spinner" />
                    </div>
                }
                {
                    status === 'open' &&
                    <div>
                        <span>{getResource('NotReady')}</span> <i className="fa fa-hourglass-start" />
                    </div>
                }
            </li>
        );
    }

    render() {
        return (
            <div className="process">
                <div class="title">Evaluation Process</div>
                <ul className="evaluation-process">
                    {this.props.processes.map(this.renderProcess)}

                </ul>
            </div>
        );
    }
}