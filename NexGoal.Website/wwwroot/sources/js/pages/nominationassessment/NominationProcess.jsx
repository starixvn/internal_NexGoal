import React, { PureComponent, Fragment } from 'react';
import moment from 'moment';

import getResource from '../../resourceHelper';
import { DateFormat } from '../../constants';

export default class NominationProcess extends PureComponent {
    render() {
        let period = this.props.period;
        let today = moment(new Date()),
            startDateForNominator = moment(period.startDateForNominator),
            endDateForNominator = moment(period.endDateForNominator),
            endDateForCommittee = moment(period.endDateForCommittee),
            startDateForComittee = moment(period.endDateForNominator).add(1, 'day');

        return (
            <ul className="evaluation-process">
                <li className={`process-item ${startDateForNominator <= today && endDateForNominator >= today ? 'in-process' : endDateForNominator < today ? 'end-process' : 'open'}`}>
                    {`${getResource('Nominator')}: ${startDateForNominator.format(DateFormat)} - ${endDateForNominator.format(DateFormat)}`}
                    {
                        endDateForNominator < today &&
                        <div>
                            <span>{getResource('Completed')}</span> <i className="fa fa-check-square-o" />
                        </div>
                    }
                    {
                        endDateForNominator >= today &&
                        <div>
                            <span>{getResource('InProgress')}</span> <i className="fa fa-spinner" />
                        </div>
                    }
                </li>
                <li className={`process-item ${endDateForNominator < today && endDateForCommittee >= today ? 'in-process' : endDateForCommittee < today ? 'end-process' : 'open'}`}>
                    {`${getResource('Committee')}: ${startDateForComittee.format(DateFormat)} - ${endDateForCommittee.format(DateFormat)}`}
                    <div>
                        {
                            endDateForCommittee < today &&
                            <div>
                                <span>{getResource('Completed')}</span> <i className="fa fa-check-square-o" />
                            </div>
                        }
                        {
                            endDateForCommittee >= today && startDateForComittee <= today &&
                            <div>
                                <span>{getResource('InProgress')}</span> <i className="fa fa-spinner" />
                            </div>
                        }
                        {
                            endDateForNominator >= today &&
                            <div>
                                <span>{getResource('NotReady')}</span> <i className="fa fa-hourglass-start" />
                            </div>
                        }
                    </div>
                </li>
            </ul>
        );
    }
}