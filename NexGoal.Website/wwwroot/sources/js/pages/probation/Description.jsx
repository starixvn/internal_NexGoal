import React, { PureComponent } from 'react';
import { DateFormat } from '../../constants';

export default class Description extends PureComponent {
    render() {
        return (
            <div className="info-rate">
                <div className="user-info">
                    <img src={this.props.employeeInfo.avatar} alt="avatar" className="avatar" />
                    <ul>
                        <li className="info">
                            <span className="label">Full Name:</span>
                            <span className="info-text">{this.props.employeeInfo.fullName}</span>
                        </li>
                        <li className="info">
                            <span className="label">Nick Name:</span>
                            <span className="info-text">{this.props.employeeInfo.nickName}</span>
                        </li>
                        <li className="info">
                            <span className="label">Joined Day:</span>
                            <span className="info-text">{this.props.employeeInfo.joinedDate.format(DateFormat)}</span>
                        </li>
                        <li className="info">
                            <span className="label">Team:</span>
                            <span className="info-text">{this.props.employeeInfo.groupName}</span>
                        </li>
                        <li className="info">
                            <span className="label">Job Title:</span>
                            <span className="info-text">{this.props.employeeInfo.jobTitle}</span>
                        </li>
                    </ul>
                </div>

                <div className="rate-information">
                    <div className="rate-item">
                        <div className="rate-star">
                            <span>Poor: 1 </span>
                            <div className="star-block">
                                <i className="fa fa-star" />
                            </div>
                        </div>
                        <div className="rate-description">Unsatisfactory: Performance was consistently below expectations. Poor quality of work, frequently needs rework. Significant improvement is needed.</div>
                    </div>
                    <div className="rate-item">
                        <div className="rate-star">
                            <span>Below Average: 2</span>
                            <div className="star-block">
                                <i className="fa fa-star" /><i className="fa fa-star" />
                            </div>
                        </div>
                        <div className="rate-description">Improvement needed: Performance did not consistently meet expectations. Did not generally contribute extra effort or energy. An improvement plan is needed.</div>
                    </div>
                    <div className="rate-item">
                        <div className="rate-star">
                            <span>Sastisfactory: 3</span>
                            <div className="star-block">
                                <i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" />
                            </div>
                        </div>
                        <div className="rate-description">Meets expectations: Solid performance with a good quality of work, at times possibly exceeding expectations and contributing additional value.</div>
                    </div>
                    <div className="rate-item">
                        <div className="rate-star">
                            <span>Good: 4</span>
                            <div className="star-block">
                                <i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" />
                            </div>
                        </div>
                        <div className="rate-description">Exceeds expectations: Significant performance and exceeded expectations. The quality of work overall was excellent and was able to deal with big problems with minimal direction.</div>
                    </div>
                    <div className="rate-item">
                        <div className="rate-star">
                            <span>Excellent:5</span>
                            <div className="star-block">
                                <i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" />
                            </div>
                        </div>
                        <div className="rate-description">Exceptional: The most outstanding performance and far exceeded expectations due to exceptionally high quality of work. Made a superior contribution in support team/department objectives.</div>
                    </div>
                </div>
            </div>
        );
    }
}