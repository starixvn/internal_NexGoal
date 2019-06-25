import React from 'react';
import { Tooltip } from 'react-bootstrap';
import { fullSymbol } from './RatingSymbol';

const EvaluationDetailTooltip =
    (<Tooltip id="tool-tip" className="evaluation-detail-tooltip">
        <p>
            Evaluating the working performance by selecting the corresponding number of {fullSymbol}
            <br />
            1 {fullSymbol} equals 1.1 pts.
               <br />
            The Score value will be rounded.
            </p>
    </Tooltip>);

export default EvaluationDetailTooltip;