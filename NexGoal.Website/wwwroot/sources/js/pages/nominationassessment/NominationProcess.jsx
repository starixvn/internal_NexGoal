import React, { PureComponent, Fragment } from 'react';
import moment from 'moment';

import getResource from '../../resourceHelper';
import { DateFormat } from '../../constants';

export default class NominationProcess extends PureComponent {
    render() {
        let period = this.props.period;
        let today = moment(),
            startDateForNominator = moment(period.startDateForNominator),
            endDateForNominator = moment(period.endDateForNominator),
            endDateForCommittee = moment(period.endDateForCommittee),
            startDateForComittee = moment(period.endDateForNominator).add(1, 'day');

        return (
            <div className="process-title">
                <div className="period">{period.title}</div>
                <ul className="evaluation-process">
                    <li className={`process-item ${today.isSameOrAfter(startDateForNominator, 'day') && today.isSameOrBefore(endDateForNominator, 'day') ? 'in-process' : today.isAfter(endDateForNominator, 'day') ? 'end-process' : 'open'}`}>
                        {`${getResource('Nominator')}: ${startDateForNominator.format(DateFormat)} - ${endDateForNominator.format(DateFormat)}`}
                        {
                            today.isAfter(endDateForNominator, 'day') &&
                            <div>
                                <span>{getResource('Completed')}</span> <i className="fa fa-check-square-o" />
                            </div>
                        }
                        {
                            today.isSameOrBefore(endDateForNominator, 'day') &&
                            <div>
                                <span>{getResource('InProgress')}</span> <i className="fa fa-spinner" />
                            </div>
                        }
                    </li>
                    <li className={`process-item ${endDateForNominator.isBefore(today, 'day') && endDateForCommittee.isSameOrAfter(today, 'day') ? 'in-process' : today.isAfter(endDateForCommittee, 'day') ? 'end-process' : 'open'}`}>
                        {`${getResource('Committee')}: ${startDateForComittee.format(DateFormat)} - ${endDateForCommittee.format(DateFormat)}`}
                        <div>
                            {
                                today.isAfter(endDateForCommittee, 'day') &&
                                <div>
                                    <span>{getResource('Completed')}</span> <i className="fa fa-check-square-o" />
                                </div>
                            }
                            {
                                today.isSameOrBefore(endDateForCommittee, 'day') && today.isSameOrAfter(startDateForComittee, 'day') &&
                                <div>
                                    <span>{getResource('InProgress')}</span> <i className="fa fa-spinner" />
                                </div>
                            }
                            {
                                today.isSameOrBefore(endDateForNominator, 'day') &&
                                <div>
                                    <span>{getResource('NotReady')}</span> <i className="fa fa-hourglass-start" />
                                </div>
                            }
                        </div>
                    </li>
                </ul>
            </div>
        );
    }
}