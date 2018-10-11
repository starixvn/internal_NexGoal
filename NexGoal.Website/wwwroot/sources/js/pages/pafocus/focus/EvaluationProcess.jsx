import React, { Fragment } from 'react';
import moment from 'moment';

import getResource from '../../../resourceHelper';

const EvaluationProcess = props => {
    let period = props.period,
        today = moment(new Date()),
        focusStartDate = period && period.focusStartDate ? moment(period.focusStartDate) : null,
        focusEndDate = period && period.focusEndDate ? moment(period.focusEndDate) : null;

    return (
        <Fragment>
            {
                period &&
                <ul className="evaluation-process">
                    <li className={`process-item ${focusStartDate <= today && focusEndDate >= today ? 'in-process' : focusEndDate < today ? 'end-process' : ''}`}>
                        {`${getResource('EvaluatorProcess')}: ${focusStartDate.format('DD MMM, YYYY')} - ${focusEndDate.format('DD MMM, YYYY')}`}
                        {
                            focusEndDate < today &&
                            <div>
                                <span>{getResource('Completed')}</span><i className="fa fa-check-square-o" />
                            </div>
                        }
                        {
                            focusStartDate <= today && focusEndDate >= today &&
                            <div>
                                <span>{getResource('InProgress')}</span><i className="fa fa-spinner" />
                            </div>
                        }
                    </li>
                </ul>
            }
        </Fragment>
    );
};

export default EvaluationProcess;